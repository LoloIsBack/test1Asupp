using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7SiemensManager
{
    using System.Text.RegularExpressions;
    using System.Windows.Forms;


    public static class S7 //lolokey{siemens;s7} 
    {
        public static class Tb //lolokey{siemens;s7;toolbox} 
        {
            public static string WordBit(ref string memory, string select) // select=bit, word lolokey{siemens;s7;bit;word;offset} 
            {
                var tmp = Regex.Matches(memory, @"\d+");
                var _memory = Convert.ToInt32(tmp[0].Value);
                var ret = "toDefine";
                var sWord = Math.Floor((double)(_memory / 8)).ToString();
                var sBit = (_memory % 8).ToString();
                if (select.ToLower() == "word") { ret = sWord; }
                if (select.ToLower() == "bit") { ret = sBit; }
                return ret;
            }
            public static void WordBitOffsetMngt(ref string memory, string lineFotChecking) // lolokey{siemens;s7;bit;word;offset} 
            {
                if (!lineFotChecking.LoloRegex(@"(?i)STRUCT|DATA_BLOCK|BOOL|INT|WORD")) return;
                var tmpMemory = memory;
                if (tmpMemory == null || tmpMemory == "") { tmpMemory = "pActal:0-pNext:0"; }
                var tmp = Regex.Matches(tmpMemory, @"\d+");
                var pActual = Convert.ToInt32(tmp[0].Value);
                var pNext = Convert.ToInt32(tmp[1].Value);

                if (lineFotChecking.LoloRegex(@": BOOL ;"))
                {
                    pActual = pNext; pNext++;
                }
                if (lineFotChecking.LoloRegex(@": (INT|WORD) ;"))
                {
                    pActual = pNext;
                    while (pActual % (2 * 8) != 0) { pActual++; }
                    pNext = pActual + 8;
                }
                if (lineFotChecking.LoloRegex(@"DATA_BLOCK")) { pActual = 0; pNext = 0; }
                if (lineFotChecking.LoloRegex(@"END_STRUCT ;"))
                {
                    while (pActual % (2 * 8) != 0) { pActual++; }
                    pNext = pActual;
                }
                memory = string.Format("pActal:{0}-pNext:{1}", pActual, pNext);
            }

        }

        public struct Importation
        {
                public static string SymbolicsTable;
        }

        public static class SymbolicsTable
        {
            public class MaterialChecker {
                public string rack, slot, type, address;
                public MaterialChecker(string _rack, string _slot, string _type, string _address) {
                     rack = _rack; slot = _slot; type = _type; address = _address;
                }
            }

            public static class Macros
            {
                public static void Importation()
                {
                   var SymbolTable = S7SiemensManager.Tb.ClipboardGetModal("Copy ALL table of 'Symbol Editor' of 'SIMATIC Manager'\ninside clipboard.\n\nnote: CTRL+A then CTRL+C");
                    var sensors = SymbolTable.LoloRegexMatches(@"\n\d+B\d.*");
                    var machines = SymbolTable.LoloRegexMatches(@"(?i).*[EAIQ]\s*\d+\.\d.*machine.*"); //tches(@"(?i).*[EA]\s*\.\d.*machine.*");
                    var circuitbreaker = SymbolTable.LoloRegexMatches(@"(?i).*[EI]\s*\d+\.\d.*breaker.*"); //tches(@"(?i).*[EA]\s*\.\d.*machine.*");
                }

                
                public static void MaterialsSchemaChecker()
                {
                    DialogResult dialogResult;
                    dialogResult = MessageBox.Show("IN: (1s1)ClipboardTableMnemoniques"+"\n"+
                        "OUT: MsgbxListDeMateriel "+"\n\n"+
                        "A partir de la table des mnemoniques, on en deduit tous les materiels utilisé", "Materiel electrique", MessageBoxButtons.OKCancel);
                    if (dialogResult != DialogResult.OK) return;

                    List<SymbolicsTable.MaterialChecker> oMaterials = new List<MaterialChecker>();
                    var _mat = new SymbolicsTable.MaterialChecker("","","","");
                    MessageBox.Show(new Form() { TopMost = true }, "put symbolics data inside clipboard");
                    var data = Clipboard.GetText();
                    var rgx = Regex.Matches(data, @"(.*?)\t");
                    Match rgx2;
                    var nbElements = 0;
                    string type = "", adr = "", rack = "", slot = "", lastData = "";
                    for (var ii = 0; ii < rgx.Count; ii++)
                    {
                        rgx2 = Regex.Match(rgx[ii].Groups[1].Value, @"(?i)^([ieaq]\d+\.|[p]?[ieaq][bdw])");
                        if (rgx2.Success)
                        {
                            nbElements++;
                            rgx2 = Regex.Match(rgx[ii + 0].Groups[1].Value, @"(?i)(?<TYPE>\D+)(?<ADDR>\d+)");
                            if (rgx2.Success && (type != rgx2.Groups[1].Value || adr != rgx2.Groups[2].Value))
                            {
                                if (adr != rgx2.Groups["ADDR"].Value)
                                {
                                    type = rgx2.Groups["TYPE"].Value;
                                    adr = rgx2.Groups["ADDR"].Value;
                                }
                            }

                            rgx2 = Regex.Match(rgx[ii + 2].Groups[1].Value, @"(?i)^(?<RACK>.+?)\.(?<SLOT>\d+):");
                            if (rgx2.Success)
                            {
                                rack = rgx2.Groups["RACK"].Value;
                                slot = rgx2.Groups["SLOT"].Value;
                            }
                            ii += 2;
                        }
                        var newLastData = $"{rack}{slot}";
                        if (lastData != "" && newLastData != lastData)
                        {
                            nbElements = 1;
                            oMaterials.Add(_mat);//new MaterialChecker( rack, slot, type, adr )
                            _mat = new MaterialChecker("", "", "", "");
                        }
                        else
                        {
                            if (nbElements > 1)
                            {
                                _mat.rack = rack; _mat.slot = slot; _mat.type = $"{type}({nbElements}x)"; _mat.address = adr;
                            }
                            else
                            {
                                _mat.rack = rack; _mat.slot = slot; _mat.type = type; _mat.address = adr;
                            }
                        }
                        lastData = newLastData;
                    }
                    oMaterials.Add(_mat);

                    List<string> allTypes = new List<string>();
                    List<string> allRacks = new List<string>();
                    foreach (var item in oMaterials)
                    {      
                        if (allRacks.FindIndex(e => e == item.rack)<0)
                            allRacks.Add(item.rack);
                    }
                    foreach (var item in oMaterials)
                    {
                        if (allTypes.FindIndex(e => e == item.type) < 0)
                            allTypes.Add(item.type);
                    }

                    var msg = "";
                    foreach (var iType in allTypes)
                    {
                        msg = $"{msg}\n====> {iType}\n";
                        foreach (var iRack in allRacks)
                        {
                            var tmp = "";
                            foreach (var item in oMaterials)
                            {
                                if (item.type == iType && item.rack == iRack) { tmp = $"{tmp}{item.slot},"; }
                            }
                            //if (iRack=="RS4") {
                            //}
                            if(tmp!="") msg = $"{msg}{iRack}: {S7SiemensManager.Tb.Range(S7SiemensManager.Tb.Range(tmp),"","")}\n";
                        }
                        }

                   
                    foreach (var item in allTypes) {
                       // msg = $"{msg}{item[0]}\n{item[1]}\n\n";
                    }
                    MessageBox.Show(new Form() { TopMost = true }, msg, "moveMe");

                    foreach (var item in oMaterials) {
                        S7SiemensManager.Tb.FindAndMoveMsgBox(0, 0, true, "moveMe");
                        MessageBox.Show(new Form() { TopMost = true }, $"rack: {item.rack}  slot: {item.slot}\ntype: {item.type}  @: {item.address}", "moveMe");
                    }

                    
                }
            }
        }

        public static class Db
        {
            
            public static class Macros {
                public static void CommentsCleaner() {  S7.Awl.Read(S7.Db.AwlLineMngt.CommentsCleaner); }

                public static void MotorsCleaner() {
                    if (Gentool.MyProject.Infos_MotorsTotal() == 0) { MessageBox.Show("No motors defined!"); }
                    else { S7.Awl.Read(S7.Db.AwlLineMngt.MotorsCleaner); } }
            }
            private  static class AwlLineMngt
            {
                public static void CommentsCleaner(S7.Awl.CFileData oAwl)
                {
                    if (oAwl.IsValueDefinition == true) { if (oAwl.Line.LoloRegex(@":.*;(\s|/)*$")) { oAwl.Line = oAwl.Line.LoloRegex(@"(^\s*)\w+", string.Format("zz_{0:00}_{1}", Convert.ToInt16(S7.Tb.WordBit(ref oAwl.MemAdd, "word")), S7.Tb.WordBit(ref oAwl.MemAdd, "bit"))); } }
                    if (oAwl.Value == Awl.EValue.Actual && oAwl.Line.LoloRegex(@"^[^:]+:=")) { oAwl.Line = ""; }
                }
                public static void MotorsCleaner(S7.Awl.CFileData oAwl)
                {
                    var MotorsPatern = Gentool.MyProject.MotorsRegexPattern();
                    if (oAwl.IsValueDefinition == true) { if (oAwl.Line.LoloRegex(@"\D\d+M\d") && !oAwl.Line.LoloRegex(MotorsPatern)) { oAwl.Line = oAwl.Line.LoloRegex(@"(^\s*)\w+", string.Format("zz_{0:00}_{1}", Convert.ToInt16(S7.Tb.WordBit(ref oAwl.MemAdd, "word")), S7.Tb.WordBit(ref oAwl.MemAdd, "bit"))); } }
                    if (oAwl.Value == Awl.EValue.Actual && oAwl.Line.LoloRegex(@"^[^:]+:=")) { oAwl.Line = ""; }
                }
            }
        }
        public  class Awl
        {
            public class CFileData {
                public string Line;
                public EType Type;
                public EValue Value;
                public bool IsValueDefinition;
                public string MemAdd;

                public CFileData() { InitObj(); }
                public void InitObj() {
                    this.Type = EType.Unknow;
                    this.Value = EValue.Unknow;
                }

            }
            public enum EType : byte { Unknow, Db, Di, Fc, Fb, Ob };
            public enum EValue : byte { Unknow, Initial, Actual, Online };
            
            public int LinePos;
            public string Name;
            public string Memory;
           

            public static void Read(Action<S7.Awl.CFileData>  func)
            {
                 var oAwl = new S7.Awl.CFileData();
                var awlNew = "";
              
                var dataStr = S7SiemensManager.Tb.ClipboardGetModal("Copy ALL AWL of 'xxx' of 'xxx'\ninside clipboard.\n\nnote: CTRL+A then CTRL+C");
                var lines = Regex.Split(dataStr, @"\r?\n");
                foreach (var line in lines)
                {
                    //Console.WriteLine(line);
                    oAwl.IsValueDefinition = false;
                    if (oAwl.Type == EType.Db && oAwl.Value == EValue.Initial && line.LoloRegex(@"^\s*\w+\s*:\s*\w")) { oAwl.IsValueDefinition = true; }
                    if (line.LoloRegex(@"^\s*DATA_BLOCK DB")) { oAwl.Type = EType.Db; }
                    if (line.LoloRegex(@"^\s*FUNCTION_BLOCK FB")) { oAwl.Type = EType.Fb; }
                    if (line.LoloRegex(@"^\s*FUNCTION FC")) { oAwl.Type = EType.Fc; }
                    if (line.LoloRegex(@"^\s*DATA_BLOCK DB")) { oAwl.Type = EType.Db; }
                    if (line.LoloRegex(@"^\s*END_[^S]")) { oAwl.InitObj(); }

                    if (oAwl.Type == EType.Db && oAwl.Value == EValue.Unknow) { if (line.LoloRegex(@"^\s*STRUCT\W")) { oAwl.Value = EValue.Initial; } }
                    if (oAwl.Type == EType.Db && oAwl.Value == EValue.Initial)
                    { if (line.LoloRegex(@"^\s*BEGIN(\W|$)")) { oAwl.Value = EValue.Actual; }}
                    oAwl.Line = line;
                    S7.Tb.WordBitOffsetMngt(ref oAwl.MemAdd, line);
                    func(oAwl);
                   
                    if (oAwl.Line != "") { awlNew += oAwl.Line + Environment.NewLine; }
                }
                Clipboard.SetText(awlNew);
                MessageBox.Show(new Form { TopMost = true }, "nouveau Awl dans le presse-papier", "lolo is back");
            }

            public static void ToDo(S7.Awl.CFileData oAwl) // easier syntax //https://social.msdn.microsoft.com/Forums/vstudio/en-US/a1ac99ed-0801-4773-a866-4223d42422cf/how-to-pass-function-as-an-argument-to-another-function-in-c?forum=csharpgeneral
            {
                if (oAwl.IsValueDefinition == true) {
                    oAwl.Line = string.Format("zz_{0:00}_{1}", S7.Tb.WordBit(ref oAwl.MemAdd, "word"), S7.Tb.WordBit(ref oAwl.MemAdd, "bit")); }
                if (oAwl.Value==EValue.Actual && oAwl.Line.LoloRegex(@"^[^:]+:=")) {oAwl.Line = ""; } //
                //Console.WriteLine(s);
                //return s;
            }

          
            }
    }
}
