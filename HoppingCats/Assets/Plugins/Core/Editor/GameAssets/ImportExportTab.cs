using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace moonNest.editor
{
    internal class ImportExportTab : TabContent
    {
        const string xlsxFormat = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        const string definitionSheetName = "_DEFINITION_";
        const string chestSheetName = "_CHEST_";
        const string battlePassSheetName = "_BATTLE_PASS_";
        const string arenaSheetName = "_ARENA_";

        readonly ExportConfig exportConfig;

        public ImportExportTab()
        {
            exportConfig = EditorConfigAsset.Ins.exportConfig;
        }

        #region editor
        public override void DoDraw()
        {
            Undo.RecordObject(EditorConfigAsset.Ins, "Editor Config");

            DrawFileInfo();
            Draw.Space(40);
            DrawExportConfig();

            if(GUI.changed)
                Draw.SetDirty(EditorConfigAsset.Ins);
        }

        void DrawFileInfo()
        {
            exportConfig.exportFileId = Draw.TextField("File Id", exportConfig.exportFileId, 300);
            Draw.Space();
            Draw.BeginHorizontal();
            if(Draw.FitButton("Create New"))
            {
                CreateNewFile();
            }

            if(exportConfig.exportFileId.Length > 0 && Draw.FitButton("Open File"))
                Application.OpenURL("https://docs.google.com/spreadsheets/d/" + exportConfig.exportFileId);

            Draw.EndHorizontal();
        }

        private async void CreateNewFile()
        {
            var newFileId = await GoogleDriveHelper.CreateFile(xlsxFormat);
            if(newFileId != null) exportConfig.exportFileId = newFileId;
        }

        void DrawExportConfig()
        {
            exportConfig.exportChest = Draw.ToggleField("Export Chest", exportConfig.exportChest, 120);
            exportConfig.exportArena = Draw.ToggleField("Export Arena", exportConfig.exportArena, 120);
            exportConfig.exportBattlePass = Draw.ToggleField("Export Battle Pass", exportConfig.exportBattlePass, 120);
            Draw.Space();
            Draw.BeginHorizontal();
            if(exportConfig.exportFileId.Length > 0)
            {
                if(Draw.FitButton("Import"))
                {
                    var stream = GoogleDriveHelper.Import(exportConfig.exportFileId);
                    if(stream != null) Import(stream);
                }

                if(Draw.FitButton("Export"))
                {
                    var stream = Export();
                    GoogleDriveHelper.Export(exportConfig.exportFileId, stream, xlsxFormat);
                }
            }
            Draw.EndHorizontal();
        }
        #endregion

        #region import methods
        void Import(Stream stream)
        {

        }
        #endregion

        #region export methods
        MemoryStream Export()
        {
            var memoryStream = new MemoryStream();
            var ep = new ExcelPackage(memoryStream);
            ExportDefinition(ep);
            if(exportConfig.exportChest) ExportChest(ep);
            if(exportConfig.exportBattlePass) ExportBattlePass(ep);
            if(exportConfig.exportArena) ExportArena(ep);
            ep.Save();
            return memoryStream;
        }

        void ExportDefinition(ExcelPackage ep)
        {
            var sheet = ep.Workbook.Worksheets.Add(definitionSheetName);
            sheet.Cells[1, 1].Value = "0,0";
            sheet.Cells[1, 2].Value = "test upload 2";
        }

        void ExportArena(ExcelPackage ep)
        {
            var sheet = ep.Workbook.Worksheets.Add(arenaSheetName);
        }

        void ExportBattlePass(ExcelPackage ep)
        {
            var sheet = ep.Workbook.Worksheets.Add(battlePassSheetName);
        }

        void ExportChest(ExcelPackage ep)
        {
            var sheet = ep.Workbook.Worksheets.Add(chestSheetName);
            sheet.Column(1).Width = 30;
            sheet.Cells[1, 1].Value = "::Name::";
            sheet.Column(2).Width = 40;
            sheet.Cells[1, 2].Value = "::Display Name::";
            sheet.Column(3).Width = 10;
            sheet.Cells[1, 3].Value = "::Layer::";
            sheet.Column(4).Width = 25;
            sheet.Cells[1, 4].Value = "::Tracking Id::";

            ExcelRange cell;
            int headerCol = 5;
            int maxCurrency = ChestAsset.Ins.chests.Max(chest => chest.content.currencies.Count);
            for(int i = 0; i < maxCurrency; i++)
            {
                var col = headerCol + i * 2;
                cell = sheet.Cells[1, col, 1, col + 1];
                cell.Merge = true;
                cell.Value = "::Currency::";
            }

            headerCol += maxCurrency * 2;
            int maxItem = ChestAsset.Ins.chests.Max(chest => chest.content.items.Count);
            for(int i = 0; i < maxItem; i++)
            {
                var col = headerCol + i * 2;
                cell = sheet.Cells[1, col, 1, col + 1];
                cell.Merge = true;
                cell.Value = "::Item::";
            }
            headerCol += maxItem * 2;
            cell = sheet.Cells[1, 1, 1, headerCol];
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.Font.Bold = true; 

            //ChestAsset.Ins.chests.ForEach((chest, i) =>
            //{
            //    if(chest.content)
            //        sheet.Cells[$"A{startRow + i}"].Value = chest.name;
            //});
        }

        void UpdateRewardHeader(ExcelWorksheet sheet, int rewardCount)
        {

        }


        #endregion
    }
}