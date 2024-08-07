using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace FW_StorageM.Pages
{
    public class ReportStockChangedItemsModel : PageModel
    {
        public List<WarehouseStockInRowData> WarehouseStockInRowDataList = new List<WarehouseStockInRowData>();
        public List<WarehouseStockOutRowData> WarehouseStockOutRowDataList = new List<WarehouseStockOutRowData>();

        public List<WarehouseStockInRowData> WarehouseWasteStockInRowDataList = new List<WarehouseStockInRowData>();
        public List<WarehouseStockOutRowData> WarehouseWasteStockOutRowDataList = new List<WarehouseStockOutRowData>();

        public Dictionary<string, List<DetailedStockItem>> WarehouseAvailableDetailedStockDictionary = new Dictionary<string, List<DetailedStockItem>>();

        public Dictionary<string, List<DetailedStockItem>> WarehouseWasteStockDictionary = new Dictionary<string, List<DetailedStockItem>>();


        public List<string> WarehouseStockChangedItemsFormworkName = new List<string>();
        
        [BindProperty]
        public DateTime SearchSrartDateFromInput { get; set; } = DateTime.Today;

        [BindProperty]
        public DateTime SearchEndDateFromInput { get; set; } = DateTime.Today.AddDays(1);
        [BindProperty]
        public string WarehouseFromInput { get; set; } = "WarehouseOne";
        public List<SelectListItem> Warehouses { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "WarehouseOne", Text = "一廠" },
            new SelectListItem { Value = "WarehouseTwo", Text = "二廠" },
            new SelectListItem { Value = "WarehouseThree", Text = "三廠"  },
        };

        public int ChangedItemsCount = 0;

        private SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
        {
            
        };


        public async Task<IActionResult> OnPostAsync()
        {
            SearchSrartDate = new DateTimeOffset(SearchSrartDateFromInput);
            SearchEndDate = new DateTimeOffset(SearchEndDateFromInput);
            Warehouse = WarehouseFromInput;

            string connectionString = builder.ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                WarehouseStockChangedItemsFormworkName.Clear();

                string DataTableWarehouseStockIn = "WarehouseTestStockIn";
                switch (Warehouse)
                {
                    case "WarehouseOne":
                        DataTableWarehouseStockIn = "WarehouseOneStockIn";
                        break;
                    case "WarehouseTwo":
                        DataTableWarehouseStockIn = "WarehouseTwoStockIn";
                        break;
                    case "WarehouseThree":
                        DataTableWarehouseStockIn = "WarehouseThreeStockIn";
                        break;
                    default:
                        DataTableWarehouseStockIn = "WarehouseTestStockIn";
                        break;
                }

                string queryStringInChangedItems = "SELECT FormworkName FROM " + DataTableWarehouseStockIn + " Where RecordTime >= @SearchSrartDate AND RecordTime <= @SearchEndDate";

                using (SqlCommand command = new SqlCommand(queryStringInChangedItems, connection))
                {
                    command.Parameters.AddWithValue("@SearchSrartDate", SearchSrartDate);
                    command.Parameters.AddWithValue("@SearchEndDate", SearchEndDate);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string ChangedItemFormworkName = reader.GetString(0);
                            if (!WarehouseStockChangedItemsFormworkName.Contains(ChangedItemFormworkName))
                                WarehouseStockChangedItemsFormworkName.Add(ChangedItemFormworkName);
                        }
                    }
                }

                string DataTableWarehouseStockOut = "WarehouseTestStockOut";
                switch (Warehouse)
                {
                    case "WarehouseOne":
                        DataTableWarehouseStockOut = "WarehouseOneStockOut";
                        break;
                    case "WarehouseTwo":
                        DataTableWarehouseStockOut = "WarehouseTwoStockOut";
                        break;
                    case "WarehouseThree":
                        DataTableWarehouseStockOut = "WarehouseThreeStockOut";
                        break;
                    default:
                        DataTableWarehouseStockOut = "WarehouseTestStockOut";
                        break;
                }

                string queryStringOutChangedItems = "SELECT FormworkName FROM " + DataTableWarehouseStockOut + " Where RecordTime >= @SearchSrartDate AND RecordTime <= @SearchEndDate";

                using (SqlCommand command = new SqlCommand(queryStringOutChangedItems, connection))
                {
                    command.Parameters.AddWithValue("@SearchSrartDate", SearchSrartDate);
                    command.Parameters.AddWithValue("@SearchEndDate", SearchEndDate);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string ChangedItemFormworkName = reader.GetString(0);
                            if (!WarehouseStockChangedItemsFormworkName.Contains(ChangedItemFormworkName))
                                WarehouseStockChangedItemsFormworkName.Add(ChangedItemFormworkName);
                        }
                    }
                }

                ChangedItemsCount = WarehouseStockChangedItemsFormworkName.Count;


                string queryStringIn = "SELECT Id, RecordTime, RecordUser, StockArea, StockLocation, FormworkName, FormworkType, SPCode, Width1, Width2, Width3, Height, Quantity, FormworkSourceLevel1, FormworkSourceLevel2, FormworkSourceLevel3, FormworkSourceLevel4, Mark FROM " + DataTableWarehouseStockIn;
                // queryStringIn += " Where RecordTime >= @SearchSrartDate AND RecordTime <= @SearchEndDate";

                using (SqlCommand command = new SqlCommand(queryStringIn, connection))
                {
                    // command.Parameters.AddWithValue("@SearchSrartDate", SearchSrartDate);
                    // command.Parameters.AddWithValue("@SearchEndDate", SearchEndDate);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        WarehouseStockInRowDataList.Clear();
                        while (reader.Read())
                        {
                            WarehouseStockInRowData tempWarehouseStockInRowData = new WarehouseStockInRowData();
                            tempWarehouseStockInRowData.Id = (int)reader.GetSqlInt32(0);
                            tempWarehouseStockInRowData.RecordTime = reader.GetDateTimeOffset(1);
                            tempWarehouseStockInRowData.RecordUser = reader.GetString(2);
                            tempWarehouseStockInRowData.StockArea = reader.GetString(3);
                            tempWarehouseStockInRowData.StockLocation = reader.GetString(4);
                            tempWarehouseStockInRowData.FormworkName = reader.GetString(5);
                            tempWarehouseStockInRowData.FormworkType = reader.GetString(6);
                            tempWarehouseStockInRowData.SPCode = reader.GetString(7);
                            tempWarehouseStockInRowData.Width1 = (int)reader.GetSqlInt32(8);
                            tempWarehouseStockInRowData.Width2 = (int)reader.GetSqlInt32(9);
                            tempWarehouseStockInRowData.Width3 = (int)reader.GetSqlInt32(10);
                            tempWarehouseStockInRowData.Height = (int)reader.GetSqlInt32(11);
                            tempWarehouseStockInRowData.Quantity = (int)reader.GetSqlInt32(12);
                            tempWarehouseStockInRowData.FormworkSourceLevel1 = reader.GetString(13);
                            tempWarehouseStockInRowData.FormworkSourceLevel2 = reader.GetString(14);
                            tempWarehouseStockInRowData.FormworkSourceLevel3 = reader.GetString(15);
                            tempWarehouseStockInRowData.FormworkSourceLevel4 = reader.GetString(16);
                            tempWarehouseStockInRowData.Mark = reader.GetString(17);

                            if (tempWarehouseStockInRowData.StockArea != "�o��")
                            {
                                WarehouseStockInRowDataList.Add(tempWarehouseStockInRowData);
                            }
                            else
                            {
                                WarehouseWasteStockInRowDataList.Add(tempWarehouseStockInRowData);
                            }
                        }
                    }
                }

                string queryStringOut = "SELECT Id, RecordTime, RecordUser, StockArea, StockLocation, FormworkName, FormworkType, SPCode, Width1, Width2, Width3, Height, Quantity, FormworkDestinationLevel1, FormworkDestinationLevel2, FormworkDestinationLevel3, FormworkDestinationLevel4, Mark FROM " + DataTableWarehouseStockOut;
                // queryStringOut += " Where RecordTime >= @SearchSrartDate AND RecordTime <= @SearchEndDate";

                using (SqlCommand command = new SqlCommand(queryStringOut, connection))
                {
                    // command.Parameters.AddWithValue("@SearchSrartDate", SearchSrartDate);
                    // command.Parameters.AddWithValue("@SearchEndDate", SearchEndDate);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        WarehouseStockOutRowDataList.Clear();
                        while (reader.Read())
                        {
                            WarehouseStockOutRowData tempWarehouseStockOutRowData = new WarehouseStockOutRowData();
                            tempWarehouseStockOutRowData.Id = (int)reader.GetSqlInt32(0);
                            tempWarehouseStockOutRowData.RecordTime = reader.GetDateTimeOffset(1);
                            tempWarehouseStockOutRowData.RecordUser = reader.GetString(2);
                            tempWarehouseStockOutRowData.StockArea = reader.GetString(3);
                            tempWarehouseStockOutRowData.StockLocation = reader.GetString(4);
                            tempWarehouseStockOutRowData.FormworkName = reader.GetString(5);
                            tempWarehouseStockOutRowData.FormworkType = reader.GetString(6);
                            tempWarehouseStockOutRowData.SPCode = reader.GetString(7);
                            tempWarehouseStockOutRowData.Width1 = (int)reader.GetSqlInt32(8);
                            tempWarehouseStockOutRowData.Width2 = (int)reader.GetSqlInt32(9);
                            tempWarehouseStockOutRowData.Width3 = (int)reader.GetSqlInt32(10);
                            tempWarehouseStockOutRowData.Height = (int)reader.GetSqlInt32(11);
                            tempWarehouseStockOutRowData.Quantity = (int)reader.GetSqlInt32(12);
                            tempWarehouseStockOutRowData.FormworkDestinationLevel1 = reader.GetString(13);
                            tempWarehouseStockOutRowData.FormworkDestinationLevel2 = reader.GetString(14);
                            tempWarehouseStockOutRowData.FormworkDestinationLevel3 = reader.GetString(15);
                            tempWarehouseStockOutRowData.FormworkDestinationLevel4 = reader.GetString(16);
                            tempWarehouseStockOutRowData.Mark = reader.GetString(17);

                            if (tempWarehouseStockOutRowData.StockArea != "�o��")
                            {
                                WarehouseStockOutRowDataList.Add(tempWarehouseStockOutRowData);
                            }
                            else
                            {
                                WarehouseWasteStockOutRowDataList.Add(tempWarehouseStockOutRowData);
                            }
                        }
                    }
                }
            }

            WarehouseAvailableDetailedStockDictionary.Clear();
            WarehouseWasteStockDictionary.Clear();

            foreach (WarehouseStockInRowData tempIn in WarehouseStockInRowDataList)
            {
                if (!WarehouseAvailableDetailedStockDictionary.ContainsKey(tempIn.FormworkName))
                {
                    WarehouseAvailableDetailedStockDictionary[tempIn.FormworkName] = new List<DetailedStockItem>();
                }

                DetailedStockItem tempDetailedStockItem = new DetailedStockItem
                {
                    FormworkName = tempIn.FormworkName,
                    FormworkType = tempIn.FormworkType
                };

                WarehouseItem warehouseItem = new WarehouseItem
                {
                    SPCode = tempIn.SPCode,
                    StockArea = tempIn.StockArea,
                    StockLocation = tempIn.StockLocation,
                    Mark = tempIn.Mark,
                    Quantity = tempIn.Quantity,
                    Width1 = tempIn.Width1,
                    Width2 = tempIn.Width2,
                    Width3 = tempIn.Width3,
                    Height = tempIn.Height
                };

                tempDetailedStockItem.WarehouseItems.Add(warehouseItem);

                WarehouseAvailableDetailedStockDictionary[tempIn.FormworkName].Add(tempDetailedStockItem);
            }

            foreach (WarehouseStockOutRowData tempOut in WarehouseStockOutRowDataList)
            {
                if (!WarehouseAvailableDetailedStockDictionary.ContainsKey(tempOut.FormworkName))
                {
                    WarehouseAvailableDetailedStockDictionary[tempOut.FormworkName] = new List<DetailedStockItem>();
                }

                DetailedStockItem tempDetailedStockItem = new DetailedStockItem
                {
                    FormworkName = tempOut.FormworkName,
                    FormworkType = tempOut.FormworkType
                };

                WarehouseItem warehouseItem = new WarehouseItem
                {
                    SPCode = tempOut.SPCode,
                    StockArea = tempOut.StockArea,
                    StockLocation = tempOut.StockLocation,
                    Mark = tempOut.Mark,
                    Quantity = tempOut.Quantity * (-1),
                    Width1 = tempOut.Width1,
                    Width2 = tempOut.Width2,
                    Width3 = tempOut.Width3,
                    Height = tempOut.Height
                };

                tempDetailedStockItem.WarehouseItems.Add(warehouseItem);

                WarehouseAvailableDetailedStockDictionary[tempOut.FormworkName].Add(tempDetailedStockItem);
            }

            foreach (WarehouseStockInRowData tempIn in WarehouseWasteStockInRowDataList)
            {
                if (!WarehouseWasteStockDictionary.ContainsKey(tempIn.FormworkName))
                {
                    WarehouseWasteStockDictionary[tempIn.FormworkName] = new List<DetailedStockItem>();
                }

                DetailedStockItem tempDetailedStockItem = new DetailedStockItem
                {
                    FormworkName = tempIn.FormworkName,
                    FormworkType = tempIn.FormworkType
                };

                WarehouseItem warehouseItem = new WarehouseItem
                {
                    SPCode = tempIn.SPCode,
                    StockArea = tempIn.StockArea,
                    StockLocation = tempIn.StockLocation,
                    Mark = tempIn.Mark,
                    Quantity = tempIn.Quantity,
                    Width1 = tempIn.Width1,
                    Width2 = tempIn.Width2,
                    Width3 = tempIn.Width3,
                    Height = tempIn.Height
                };

                tempDetailedStockItem.WarehouseItems.Add(warehouseItem);

                WarehouseWasteStockDictionary[tempIn.FormworkName].Add(tempDetailedStockItem);
            }

            foreach (WarehouseStockOutRowData tempOut in WarehouseWasteStockOutRowDataList)
            {
                if (!WarehouseWasteStockDictionary.ContainsKey(tempOut.FormworkName))
                {
                    WarehouseWasteStockDictionary[tempOut.FormworkName] = new List<DetailedStockItem>();
                }

                DetailedStockItem tempDetailedStockItem = new DetailedStockItem
                {
                    FormworkName = tempOut.FormworkName,
                    FormworkType = tempOut.FormworkType
                };

                WarehouseItem warehouseItem = new WarehouseItem
                {
                    SPCode = tempOut.SPCode,
                    StockArea = tempOut.StockArea,
                    StockLocation = tempOut.StockLocation,
                    Mark = tempOut.Mark,
                    Quantity = tempOut.Quantity * (-1),
                    Width1 = tempOut.Width1,
                    Width2 = tempOut.Width2,
                    Width3 = tempOut.Width3,
                    Height = tempOut.Height
                };

                tempDetailedStockItem.WarehouseItems.Add(warehouseItem);

                WarehouseWasteStockDictionary[tempOut.FormworkName].Add(tempDetailedStockItem);
            }

            // Initialize a variable to hold the latest update time
            DateTimeOffset latestUpdateTime = DateTimeOffset.MinValue;

            // Check the latest update time in the Stock In list
            if (WarehouseStockInRowDataList.Any())
            {
                var maxInTime = WarehouseStockInRowDataList.Max(x => x.RecordTime);
                if (maxInTime > latestUpdateTime)
                {
                    latestUpdateTime = maxInTime;
                }
            }

            // Check the latest update time in the Stock Out list
            if (WarehouseStockOutRowDataList.Any())
            {
                var maxOutTime = WarehouseStockOutRowDataList.Max(x => x.RecordTime);
                if (maxOutTime > latestUpdateTime)
                {
                    latestUpdateTime = maxOutTime;
                }
            }

            // Find the Taipei Time Zone
            TimeZoneInfo taipeiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");

            // Convert latestUpdateTime to Taipei Time Zone
            DateTimeOffset taipeiTime = TimeZoneInfo.ConvertTime(latestUpdateTime, taipeiTimeZone);

            LatestUpdateTime = taipeiTime;
            // Now, latestUpdateTime contains the most recent update time across all formworks

            IsPostRequestHandled = true;

            return Page();
        }

        public class WarehouseAvailableStock
        {
            public string StockArea { get; set; } = string.Empty;
            public string StockLocation { get; set; } = string.Empty;
            public string FormworkName { get; set; } = string.Empty;
            public string FormworkType { get; set; } = string.Empty;
            public string SPCode { get; set; } = string.Empty;
            public int Width1 { get; set; }
            public int Width2 { get; set; }
            public int Width3 { get; set; }
            public int Height { get; set; }
            public int Quantity { get; set; }
            public string Mark { get; set; } = string.Empty;
        }

        public class WarehouseStockInRowData
        {
            public int Id { get; set; }
            public DateTimeOffset RecordTime { get; set; }
            public string RecordUser { get; set; } = string.Empty;
            public string StockArea { get; set; } = string.Empty;
            public string StockLocation { get; set; } = string.Empty;
            public string FormworkName { get; set; } = string.Empty;
            public string FormworkType { get; set; } = string.Empty;
            public string SPCode { get; set; } = string.Empty;
            public int Width1 { get; set; }
            public int Width2 { get; set; }
            public int Width3 { get; set; }
            public int Height { get; set; }
            public int Quantity { get; set; }
            public string FormworkSourceLevel1 { get; set; } = string.Empty;
            public string FormworkSourceLevel2 { get; set; } = string.Empty;
            public string FormworkSourceLevel3 { get; set; } = string.Empty;
            public string FormworkSourceLevel4 { get; set; } = string.Empty;
            public string Mark { get; set; } = string.Empty;
        }

        public class WarehouseStockOutRowData
        {
            public int Id { get; set; }
            public DateTimeOffset RecordTime { get; set; }
            public string RecordUser { get; set; } = string.Empty;
            public string StockArea { get; set; } = string.Empty;
            public string StockLocation { get; set; } = string.Empty;
            public string FormworkName { get; set; } = string.Empty;
            public string FormworkType { get; set; } = string.Empty;
            public string SPCode { get; set; } = string.Empty;
            public int Width1 { get; set; }
            public int Width2 { get; set; }
            public int Width3 { get; set; }
            public int Height { get; set; }
            public int Quantity { get; set; }
            public string FormworkDestinationLevel1 { get; set; } = string.Empty;
            public string FormworkDestinationLevel2 { get; set; } = string.Empty;
            public string FormworkDestinationLevel3 { get; set; } = string.Empty;
            public string FormworkDestinationLevel4 { get; set; } = string.Empty;
            public string Mark { get; set; } = string.Empty;
        }

        public class DetailedStockItem
        {
            public string FormworkName { get; set; } = string.Empty;
            public string FormworkType { get; set; } = string.Empty;
            public List<WarehouseItem> WarehouseItems { get; set; } = new List<WarehouseItem>();
        }

        public class WarehouseItem
        {
            public string SPCode { get; set; } = string.Empty;
            public string StockArea { get; set; } = string.Empty;
            public string StockLocation { get; set; } = string.Empty;
            public string Mark { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public int Width1 { get; set; }
            public int Width2 { get; set; }
            public int Width3 { get; set; }
            public int Height { get; set; }
        }

        public bool IsPostRequestHandled { get; set; } = false;
        public DateTimeOffset LatestUpdateTime { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset SearchSrartDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset SearchEndDate { get; set; } = DateTimeOffset.Now;
        public string Warehouse { get; set; } = "WarehouseOne";


        public class WarehouseStockChangedItem
        {
            public string FormworkName { get; set; } = string.Empty;
            public int Quantity { get; set; }
        }
    }
}
