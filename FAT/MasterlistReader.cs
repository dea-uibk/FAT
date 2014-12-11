using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace FAT
{
    class MasterlistReader
    {
        public static DataTable ReadMasterList(string masterListFn)
        {
            if (!File.Exists(masterListFn)) {
                throw new Exception(masterListFn + " not found!");
            }

            try {
                //throw new Exception("testing COM...");
                return readMasterListOLEDB(masterListFn);
            }
            //catch (InvalidOperationException e)
            catch (Exception e) {
                Trace.WriteLine("Cannot read masterlist with OLEB driver... using COM!");
                return ReadMasterListCom(masterListFn);
            }
        }

        public static DataTable ReadMasterListCom(string masterListFn)
        {
            Trace.WriteLine("reading masterlist using COM...");
            // benötigte Objekte vorbereiten
            Microsoft.Office.Interop.Excel.Application excel = null;
            Workbook wb = null;
            //try
            //{
            // Excel starten
            excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = false;

            // Datei öffnen
            wb = excel.Workbooks.Open(
                new FileInfo(masterListFn).FullName,
                ExcelKonstanten.UpdateLinks.DontUpdate,
                ExcelKonstanten.ReadOnly,
                ExcelKonstanten.Format.Nothing,
                "", // Passwort
                "", // WriteResPasswort
                ExcelKonstanten.IgnoreReadOnlyRecommended,
                XlPlatform.xlWindows,
                "", // Trennzeichen
                ExcelKonstanten.Editable,
                ExcelKonstanten.DontNotifiy,
                ExcelKonstanten.Converter.Default,
                ExcelKonstanten.DontAddToMru,
                ExcelKonstanten.Local,
                ExcelKonstanten.CorruptLoad.NormalLoad);

            // Arbeitsblätter lesen
            Sheets sheets = wb.Worksheets;


            // ein Arbeitsblatt auswählen…
            //Worksheet ws = (Worksheet)sheets.get_Item("owssvr");
            Worksheet ws = (Worksheet)sheets[1];
            ws.Select();

            DataTable dt = new DataTable();
            var xlRange = ws.UsedRange;
            if (xlRange != null) {
                int nRows = xlRange.Rows.Count;
                int nCols = xlRange.Columns.Count;
                //Console.Write("nRows = " + nRows);
                //Console.Write("nRows = " + nCols);

                // parse 1st row:
                var cols = new object[nCols];

                for (int j = 1; j <= nCols; ++j) {
                    var r = (Microsoft.Office.Interop.Excel.Range)ws.Cells[1, j];
                    dt.Columns.Add(r.Value2);
                }

                var rowData = new object[nCols];
                for (int i = 2; i <= nRows; ++i) {
                    for (int j = 1; j <= nCols; ++j) {
                        var r = (Microsoft.Office.Interop.Excel.Range)ws.Cells[i, j];
                        rowData[j - 1] = r.Value2;
                    }
                    dt.Rows.Add(rowData);
                }
                Trace.WriteLine("readMasterListCOM: converted masterlist to datatable!");

                //DataRow[] result = dt.Select("UID = 'KB_00031'");
                //Console.WriteLine("result size = " + result.Count());
                //foreach (DataRow r in result)
                //{
                //    //Console.WriteLine("result: ");
                //    foreach (var i in r.ItemArray)
                //    {
                //        Console.Write(i + " ");
                //    }
                //}
            }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            //finally
            //{
            //    wb.Close(false, null, null);
            //    excel.Quit();
            //}

            wb.Close(false, null, null);
            excel.Quit();

            return dt;
        }

        public static DataTable readMasterListOLEDB(string masterListFn)
        {
            Trace.WriteLine("reading masterlist using OLEDB...");
            //try
            //{
            //var connectionString =
            //string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;HDR=Yes;IMEX=1;",
            //"c:\\Masterlist.xlsx");
            //var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;'", "c:\\Masterlist.xlsx");
            var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;'", masterListFn);
            //Trace.WriteLine("readMasterListOLEDB: connectionString: " + connectionString);

            var con = new OleDbConnection(connectionString);
            var command = new OleDbCommand();
            var dt = new DataTable();
            con.Open();
            var dtSchema = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            var sheet1Name = dtSchema.Rows[0].Field<string>("TABLE_NAME");
            //Trace.WriteLine("sheet1Name = "+sheet1Name);
            var myCommand = new OleDbDataAdapter("select * from [" + sheet1Name + "]", con);
            //var myCommand = new OleDbDataAdapter("select * from [owssvr$]", con);
            myCommand.Fill(dt);
            //DataRow[] result = dt.Select("UID = 'KB_00031'");
            //Console.WriteLine("result size = "+result.Count());
            //foreach (DataRow r in result)
            //{
            //    Console.WriteLine("result: ");    
            //    foreach (var i in r.ItemArray)
            //    {
            //        Console.Write(i + " ");
            //    }
            //}
            //Console.WriteLine(); 
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Message: "+e.Message);
            //    Console.WriteLine(e.StackTrace);
            //}

            return dt;
        }

    }
}
