using System.Collections.Generic;
public static class AutoTranslate
{

public static string High_Score (string Num)  { return(Translator.inst.Translate("High_Score", new(){("Num", Num)})); }

public static string FPS (string Num)  { return(Translator.inst.Translate("FPS", new(){("Num", Num)})); }

public static string Score_Text (string Num1,string Num2)  { return(Translator.inst.Translate("Score_Text", new(){("Num1", Num1),("Num2", Num2)})); }

public static string Score_Text_No_Limit (string Num1)  { return(Translator.inst.Translate("Score_Text_No_Limit", new(){("Num1", Num1)})); }

public static string Drop_Text (string Num1,string Num2)  { return(Translator.inst.Translate("Drop_Text", new(){("Num1", Num1),("Num2", Num2)})); }

public static string Merge_Crown_Tutorial (string Num)  { return(Translator.inst.Translate("Merge_Crown_Tutorial", new(){("Num", Num)})); }

public static string Drop_Shape_Tutorial (string Num)  { return(Translator.inst.Translate("Drop_Shape_Tutorial", new(){("Num", Num)})); }

public static string Drop_Shape_WinCon (string Num)  { return(Translator.inst.Translate("Drop_Shape_WinCon", new(){("Num", Num)})); }

public static string Drop_Endless_Tutorial (string Num)  { return(Translator.inst.Translate("Drop_Endless_Tutorial", new(){("Num", Num)})); }

public static string Time (string Time)  { return(Translator.inst.Translate("Time", new(){("Time", Time)})); }

public static string DoEnum(ToTranslate thing) {return(Translator.inst.Translate(thing.ToString()));}
}
public enum ToTranslate {
All_Shapes_and_Sizes,Designer,Inspiration,Last_Update,Translator_Credit,Language,Sound_Credits,Clear_Data,Merge_Crowns,Drop_Shapes,Endless_Drops,Endless_Scoring,Basics,Moving,Bottleneck,Bouncy,Give_Up,Hide_UI,Next,You_Gave_Up,You_Lost,You_Won,Title_Screen,Replay,Begin,Tutorial_1,Tutorial_2,Tutorial_3,Merge_Crown_WinCon,Merge_Endless_Tutorial,Endless_Wincon,Saved,Update_History,Out_of_Shapes,Blank,Update_0,Update_0_Text,Upload_Translation,Download_English,Update_1,Update_1_Text}
