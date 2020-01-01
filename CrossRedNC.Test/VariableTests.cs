namespace CrossRedNC.Test
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using CrossRefNC;
    using NUnit.Framework;

    public class VariableTests
    {
        [TestCase(";R831 = Maxkraft Zo", "R831", "Maxkraft Zo")]
        [TestCase(";R831=Maxkraft Zo", "R831", "Maxkraft Zo")]
        public void IsReturningSingleVariable(string line, string expectedVariable, string expectedComment)
        {
            Assert.AreEqual(true, Variable.TryParse(line, "File.txt", out var results));
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(expectedVariable, results[0].Name);
            Assert.AreEqual(expectedComment, results[0].Comment);
        }

        [TestCase(";R831=Maxkraft Zo   R893=Förstärkningsfaktor Zo", "R831", "Maxkraft Zo", "R893", "Förstärkningsfaktor Zo")]
        [TestCase(";R831= Maxkraft Zo   R893=Förstärkningsfaktor Zo", "R831", "Maxkraft Zo", "R893", "Förstärkningsfaktor Zo")]
        public void IsReturningCorrectVariable(string line, string variable1, string comment1, string variable2, string comment2)
        {
            Assert.AreEqual(true, Variable.TryParse(line, "File.txt", out var results));
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(variable1, results[0].Name);
            Assert.AreEqual(comment1, results[0].Comment);
            Assert.AreEqual(variable2, results[1].Name);
            Assert.AreEqual(comment2, results[1].Comment);
        }

        [TestCase(@"proc forcemonitoring save
; R - parametrar för maxkrafter i Newton
;R831 = Maxkraft Zo   R893 = Förstärkningsfaktor Zo
;R832 = Maxkraft Zi   R894 = Förstärkningsfaktor Zi
;R833 = Maxkraft Wo   R896 = Förstärkningsfaktor Wo
;R834 = Maxkraft Wi   R897 = Förstärkningsfaktor Wi
;R835 = Maxkraft P    R900 = Förstärkningsfaktor P
;R836 = Maxkraft Q    R901 = Förstärkningsfaktor Q
;R837 = Maxkraft CAL


;Nya funktioner för övervakning av multiaxiell pressning. 2009 - 01 - 20 KLE
ids = 27 do $r829 = (($a_ina[1] * $R893) + ($a_ina[2] * $R894))
;Maxkraftövervakning axel Zo/Zi
;Zo i minusriktning($a_in[16]==1) eller Zi i minusriktning($a_in[18]==1) Används ej för tillfället i 2G
ids = 39 whenever($r829>=120000) do $a_out[25]=1 $R99=-888

ids=28 do $r830=(($a_ina[3] * $R896) + ($a_ina[4] * $R897))
;Maxkraftövervakning axel Wo/Wi
;Wo i plusriktning($a_in[7]==1) eller Wi i plusriktning($a_in[9]==1) Används ej för tillfället i 2G
ids = 41 whenever($r830>=120000) do $a_out[26]=1 $R99=-888

;Maxkraftövervakning axel Wo
;Wo i plusriktning($a_in[7]==1) eller Zo i minusriktning($a_in[16]==1) Används ej för tillfället i 2G
ids = 38 whenever(($R896* $a_ina[3]) >= $r833) do $a_out[28]=1 $R99=-888

;Maxkraftövervakning axel Wi
;Wi i plusriktning($a_in[9]==1) eller Zi i minusriktning($a_in[18]==1) Används ej för tillfället i 2G
ids = 40 whenever(($R897* $a_ina[4]) >= $r834) do $a_out[29]=1 $R99=-888

;Maxkraftövervakning axel Zo
;Wo i plusriktning($a_in[7]==1) eller Zo i minusriktning($a_in[16]==1) Används ej för tillfället i 2G
ids = 42 whenever(($R893* $a_ina[1]) >= $r831) do $a_out[30]=1 $R99=-888

;Maxkraftövervakning axel Zi
;Wi i plusriktning($a_in[9]==1) eller Zi i minusriktning($a_in[18]==1) Används ej för tillfället i 2G
ids = 44 whenever(($R894* $a_ina[2]) >= $r832) do $a_out[31]=1 $R99=-888

;Nya funktioner för övervakning av maxkraft P/Q(id 51/52)  2009-09-04 KLE
ids = 51 whenever((($R900* $a_ina[5]) >= $r835) or(($R901* $a_ina[6]) >= $r836)) do $a_out[38]=1 $R99=-888

ids=43 whenever($r829<($r831+$r832)) do $a_out[25]=0
ids=45 whenever($r830<($r833+$r834)) do $a_out[26]=0
ids=46 whenever(($a_ina[3]<$r833) and($a_ina[4]<$r834)) do $a_out[28]=0 $a_out[29]=0
ids=48 whenever(($a_ina[1]<$r831) and($a_ina[2]<$r832)) do $a_out[30]=0 $a_out[31]=0
ids=52 whenever($a_ina[5]<$r835) and($a_ina[6]<$r836) do $a_out[38]=0
ret
end proc
")]
        public void ReadForceMonitoring(string fileContent)
        {
            var variables = this.ReadFile(fileContent);
            Assert.AreEqual(52, variables.Count);
        }

        [TestCase(@";Hash=-2114111879
; Press.mpf
R99=1
R110=1
R111=0
R113=1
R854=0
R861=0
R862=0
R109=-97.0847
G1 G90
G64
maxf_on
maxon
BEGIN_1:
IF $a_in[22]==0 GOTOB BEGIN_1
MSG(Loopstart)
G1 G90 F1200
ZO_1 = -8.0447 ZI_1 = -13.0447 WO_1 = 84.1626 WI_1 = 78.6626 U_1 = -130.0000
MSG(Fyllning)
$SN_SW_CAM_PLUS_POS_TAB_1[4] = -5.0000
ZO_1 = -8.0447 ZI_1 = -13.0447 WO_1 = 74.5726 WI_1 = 69.0726 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F8485 ACC[WO_1] = 33
stopre
id = 14 whenever($$a_in[19] == 0) do $ac_ovr = 0
whenever($$a_ina[3] > $r854) do $r854 = $$a_ina[3]
whenever($$a_ina[5] > $r861) do $r861 = $$a_ina[5]
whenever($$a_ina[6] > $r862) do $r862 = $$a_ina[6]
ZO_1 = -8.0447 ZI_1 = -13.0447 WO_1 = 74.6726 + R365 WI_1 = 69.1726 + R366 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -20.0000 F48000 ACC[U_1] = 50
ZO_1 = -8.0447 ZI_1 = -13.0447 WO_1 = 74.6726 + R365 WI_1 = 69.1726 + R366 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = 10.0000 F48000 ACC[U_1] = 50
ZO_1 = -8.0447 ZI_1 = -13.0447 WO_1 = 74.6726 + R365 WI_1 = 69.1726 + R366 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -20.0000 F48000 ACC[U_1] = 50
ZO_1 = -8.0447 ZI_1 = -13.0447 WO_1 = 74.6726 + R365 WI_1 = 69.1726 + R366 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = 10.0000 F48000 ACC[U_1] = 50
ZO_1 = -8.0447 ZI_1 = -13.0447 WO_1 = 74.6726 + R365 WI_1 = 69.1726 + R366 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -20.0000 F48000 ACC[U_1] = 50
ZO_1 = -8.0447 ZI_1 = -13.0447 WO_1 = 74.6726 + R365 WI_1 = 69.1726 + R366 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = 10.0000 F48000 ACC[U_1] = 50
ZO_1 = -8.0447 ZI_1 = -13.0447 WO_1 = 74.6726 + R365 WI_1 = 69.1726 + R366 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -20.0000 F48000 ACC[U_1] = 50
ZO_1 = -8.0447 ZI_1 = -13.0447 WO_1 = 74.6726 + R365 WI_1 = 69.1726 + R366 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F48000 ACC[U_1] = 50
STOPRE
MSG(Äntring)
$SN_SW_CAM_MINUS_POS_TAB_1[5] = -92.5447
ZO_1 = -92.8447 ZI_1 = -97.8447 WO_1 = 73.4576 WI_1 = 67.9576 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F11966 ACC[ZO_1] = 67
ZO_1 = -92.8447 ZI_1 = -97.8447 WO_1 = 73.4576 WI_1 = 67.9576 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F1440 ACC[ZO_1] = 67
ZO_1 = -96.5097 ZI_1 = -101.5097 WO_1 = 73.4576 WI_1 = 67.9576 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F2036 ACC[ZO_1] = 67
MSG(Pressning)
komp_on
ZO_1 = -96.5097 ZI_1 = -101.5097 WO_1 = 76.3176 WI_1 = 70.8176 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F2206 ACC[WO_1] = 33
ZO_1 = -96.9097 ZI_1 = -101.9097 WO_1 = 76.3176 WI_1 = 70.8176 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F2206 ACC[ZO_1] = 33
ZO_1 = -97.1597 ZI_1 = -102.1597 WO_1 = 76.3176 WI_1 = 70.8176 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F170 ACC[ZO_1] = 33
STOPRE
G4 F0.080
R111 = 1
R99 = 2
R364 = R364 + 1
id = 11 when($va_im2[zo_1] > -29.045) do $R113 = 0
CANCEL(14)
MSG(Avlastning)
ZO_1 = -97.0847 ZI_1 = -102.0847 WO_1 = 76.3176 WI_1 = 70.8176 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F42 ACC[ZO_1] = 17 G601
MSG(Robot plockning)
IF $a_dbw[2] < 100 ;Override less than 100 %
  $SN_SW_CAM_PLUS_POS_TAB_1[7] = -8.0447 - 1.0
ELSE
  $SN_SW_CAM_PLUS_POS_TAB_1[7] = -18.0447
ENDIF

ZO_1 = -97.0347 ZI_1 = -102.0347 WO_1 = 76.3676 WI_1 = 70.8676 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F120 ACC[WO_1] = 17
ZO_1 = -90.2397 ZI_1 = -95.2397 WO_1 = 83.1626 WI_1 = 77.6626 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F3600 ACC[WO_1] = 67
ZO_1 = -88.8397 ZI_1 = -93.8397 WO_1 = 84.1626 WI_1 = 78.6626 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F3128 ACC[WO_1] = 67
ZO_1 = -8.0447 ZI_1 = -13.0447 WO_1 = 84.1626 WI_1 = 78.6626 R_1 = 0.0000 P_1 = 15.0000 Q_1 = 15.0000 U_1 = -130.0000 F12134 ACC[ZO_1] = 67
STOPRE
R99 = 97
$SN_SW_CAM_PLUS_POS_TAB_1[7] = 999
$SN_SW_CAM_MINUS_POS_TAB_1[5] = -999
R110 = 1
M2
")]
        public void ReadMpfFile(string fileContent)
        {
            var variables = this.ReadFile(fileContent);
            Assert.AreEqual(37, variables.Count);
        }

        public IReadOnlyList<Variable> ReadFile(string fileContent)
        {
            var stringReader = new StringReader(fileContent);
            List<Variable> variables = new List<Variable>();
            string line = null;
            while ((line = stringReader.ReadLine()) != null)
            {
                if (Variable.TryParse(line, "File.txt", out var results))
                {
                    variables.AddRange(results);
                }
            }

            return variables;
        }
    }
}
