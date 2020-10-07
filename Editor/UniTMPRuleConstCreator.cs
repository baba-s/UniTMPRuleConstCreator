using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
	/// <summary>
	/// TMPRule に登録されているシンボルの定数を管理するクラスを作成するエディタ拡張
	/// </summary>
	[InitializeOnLoad]
	internal static class UniTMPRuleConstCreator
	{
		//================================================================================
		// 関数(static)
		//================================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		static UniTMPRuleConstCreator()
		{
			TMPRuleSettingsInspector.OnHeaderGUI += OnHeaderGUI;
		}

		/// <summary>
		/// UniSymbolSettings の Inspector のヘッダの GUI を描画する時に呼び出されます
		/// </summary>
		private static void OnHeaderGUI( TMPRuleSettings settings )
		{
			if ( GUILayout.Button( "定数を管理するスクリプトを生成" ) )
			{
				Create( settings );
			}
		}

		/// <summary>
		/// 定数を管理するクラスを生成します
		/// </summary>
		public static void Create( TMPRuleSettings settings )
		{
			var editorSettings = UniTMPRuleConstCreatorSettings.GetInstance();
			var template       = editorSettings.CodeTemplate;

			var values = settings.List
					.Select( x => new ConstStringCodeGeneratorOptions.Element { Name = x.Name, Value = x.Name, Comment = x.Comment } )
					.ToArray()
				;

			var options = new ConstStringCodeGeneratorOptions
			{
				Template = template,
				Elements = values,
			};

			var path = editorSettings.OutputAssetPath;
			var code = ConstStringCodeGenerator.Generate( options );

			code = code
					.Replace( "\t", "    " )
					.Replace( "\r\n", "#NEW_LINE#" )
					.Replace( "\r", "#NEW_LINE#" )
					.Replace( "\n", "#NEW_LINE#" )
					.Replace( "#NEW_LINE#", "\r\n" )
				;

			ConstStringCodeGenerator.Write( path, code );
			AssetDatabase.Refresh();
		}
	}
}