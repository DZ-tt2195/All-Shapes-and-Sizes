public static class AutoTranslate 
{ 

public static string High_Score (string Num) => Translator.inst.Translate("High_Score", new() {("Num", Num)});

public static string FPS (string Num) => Translator.inst.Translate("FPS", new() {("Num", Num)});

public static string Score_Text (string Num1,string Num2) => Translator.inst.Translate("Score_Text", new() {("Num1", Num1),("Num2", Num2)});

public static string Score_Text_No_Limit (string Num1) => Translator.inst.Translate("Score_Text_No_Limit", new() {("Num1", Num1)});

public static string Drop_Text (string Num1,string Num2) => Translator.inst.Translate("Drop_Text", new() {("Num1", Num1),("Num2", Num2)});

public static string Merge_Crown_Tutorial (string Num) => Translator.inst.Translate("Merge_Crown_Tutorial", new() {("Num", Num)});

public static string Drop_Shape_Tutorial (string Num) => Translator.inst.Translate("Drop_Shape_Tutorial", new() {("Num", Num)});

public static string Drop_Shape_WinCon (string Num) => Translator.inst.Translate("Drop_Shape_WinCon", new() {("Num", Num)});

public static string Drop_Endless_Tutorial (string Num) => Translator.inst.Translate("Drop_Endless_Tutorial", new() {("Num", Num)});

public static string Time (string Time) => Translator.inst.Translate("Time", new() {("Time", Time)});

public static string All_Shapes_and_Sizes() => Translator.inst.Translate("All_Shapes_and_Sizes");
public static string Designer() => Translator.inst.Translate("Designer");
public static string Inspiration() => Translator.inst.Translate("Inspiration");
public static string Last_Update() => Translator.inst.Translate("Last_Update");
public static string Translator_Credit() => Translator.inst.Translate("Translator_Credit");
public static string Language() => Translator.inst.Translate("Language");
public static string Sound_Credits() => Translator.inst.Translate("Sound_Credits");
public static string Clear_Data() => Translator.inst.Translate("Clear_Data");
public static string Merge_Crowns() => Translator.inst.Translate("Merge_Crowns");
public static string Drop_Shapes() => Translator.inst.Translate("Drop_Shapes");
public static string Endless_Drops() => Translator.inst.Translate("Endless_Drops");
public static string Endless_Scoring() => Translator.inst.Translate("Endless_Scoring");
public static string Basics() => Translator.inst.Translate("Basics");
public static string Moving() => Translator.inst.Translate("Moving");
public static string Bottleneck() => Translator.inst.Translate("Bottleneck");
public static string Bouncy() => Translator.inst.Translate("Bouncy");
public static string Give_Up() => Translator.inst.Translate("Give_Up");
public static string Hide_UI() => Translator.inst.Translate("Hide_UI");
public static string Next() => Translator.inst.Translate("Next");
public static string You_Gave_Up() => Translator.inst.Translate("You_Gave_Up");
public static string You_Lost() => Translator.inst.Translate("You_Lost");
public static string You_Won() => Translator.inst.Translate("You_Won");
public static string Title_Screen() => Translator.inst.Translate("Title_Screen");
public static string Replay() => Translator.inst.Translate("Replay");
public static string Begin() => Translator.inst.Translate("Begin");
public static string Tutorial_1() => Translator.inst.Translate("Tutorial_1");
public static string Tutorial_2() => Translator.inst.Translate("Tutorial_2");
public static string Tutorial_3() => Translator.inst.Translate("Tutorial_3");
public static string Merge_Crown_WinCon() => Translator.inst.Translate("Merge_Crown_WinCon");
public static string Merge_Endless_Tutorial() => Translator.inst.Translate("Merge_Endless_Tutorial");
public static string Endless_Wincon() => Translator.inst.Translate("Endless_Wincon");
public static string Saved() => Translator.inst.Translate("Saved");
public static string Update_History() => Translator.inst.Translate("Update_History");
public static string Out_of_Shapes() => Translator.inst.Translate("Out_of_Shapes");
public static string Blank() => Translator.inst.Translate("Blank");
public static string Update_0() => Translator.inst.Translate("Update_0");
public static string Update_0_Text() => Translator.inst.Translate("Update_0_Text");
public static string Upload_Translation() => Translator.inst.Translate("Upload_Translation");
public static string Download_English() => Translator.inst.Translate("Download_English");
public static string Update_1() => Translator.inst.Translate("Update_1");
public static string Update_1_Text() => Translator.inst.Translate("Update_1_Text");
}
public enum ToTranslate {
All_Shapes_and_Sizes,Designer,Inspiration,Last_Update,Translator_Credit,Language,Sound_Credits,Clear_Data,Merge_Crowns,Drop_Shapes,Endless_Drops,Endless_Scoring,Basics,Moving,Bottleneck,Bouncy,Give_Up,Hide_UI,Next,You_Gave_Up,You_Lost,You_Won,Title_Screen,Replay,Begin,Tutorial_1,Tutorial_2,Tutorial_3,Merge_Crown_WinCon,Merge_Endless_Tutorial,Endless_Wincon,Saved,Update_History,Out_of_Shapes,Blank,Update_0,Update_0_Text,Upload_Translation,Download_English,Update_1,Update_1_Text
}
