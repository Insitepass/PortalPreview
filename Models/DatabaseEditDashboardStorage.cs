using DevExpress.DashboardWeb;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace PortalPreview.Models
{
    public class DatabaseEditDashboardStorage
    {
        public class DataBaseEditaleDashboardStorage : IEditableDashboardStorage
        {
            private string Multicorp_Source;

            public DataBaseEditaleDashboardStorage(string connectionString)
            {
                this.Multicorp_Source = connectionString;
            }

            public string AddDashboard(XDocument document, string dashboardName)
            {
                using (SqlConnection connection = new SqlConnection(Multicorp_Source))
                {
                    connection.Open();
                    MemoryStream stream = new MemoryStream();
                    document.Save(stream);
                    stream.Position = 0;

                    SqlCommand InsertCommand = new SqlCommand(
                        "INSERT INTO Dashboards (Dashboard, Caption) " +
                        "output INSERTED.ID " +
                        "VALUES (@Dashboard, @Caption)");
                    InsertCommand.Parameters.Add("Caption", SqlDbType.NVarChar).Value = dashboardName;
                    InsertCommand.Parameters.Add("Dashboard", SqlDbType.VarBinary).Value = stream.ToArray();
                    InsertCommand.Connection = connection;
                    string ID = InsertCommand.ExecuteScalar().ToString();
                    connection.Close();
                    return ID;
                }
            }

            public XDocument LoadDashboard(string dashboardID)
            {
                using (SqlConnection connection = new SqlConnection(Multicorp_Source))
                {
                    connection.Open();
                    SqlCommand GetCommand = new SqlCommand("SELECT  Dashboard FROM Dashboards WHERE ID=@ID");
                    GetCommand.Parameters.Add("ID", SqlDbType.Int).Value = Convert.ToInt32(dashboardID);
                    GetCommand.Connection = connection;
                    SqlDataReader reader = GetCommand.ExecuteReader();
                    reader.Read();
                    byte[] data = reader.GetValue(0) as byte[];
                    MemoryStream stream = new MemoryStream(data);
                    connection.Close();
                    return XDocument.Load(stream);
                }
            }

            public IEnumerable<DashboardInfo> GetAvailableDashboardsInfo()
            {
                List<DashboardInfo> list = new List<DashboardInfo>();
                using (SqlConnection connection = new SqlConnection(Multicorp_Source))
                {
                    connection.Open();
                    SqlCommand GetCommand = new SqlCommand("SELECT ID, Caption FROM Dashboards");
                    GetCommand.Connection = connection;
                    SqlDataReader reader = GetCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string ID = reader.GetInt32(0).ToString();
                        string Caption = reader.GetString(1);
                        list.Add(new DashboardInfo() { ID = ID, Name = Caption });
                    }
                    connection.Close();
                }
                return list;
            }

            public void SaveDashboard(string dashboardID, XDocument document)
            {
                using (SqlConnection connection = new SqlConnection(Multicorp_Source))
                {
                    connection.Open();
                    MemoryStream stream = new MemoryStream();
                    document.Save(stream);
                    stream.Position = 0;

                    SqlCommand InsertCommand = new SqlCommand(
                        "UPDATE Dashboards Set Dashboard = @Dashboard " +
                        "WHERE ID = @ID");
                    InsertCommand.Parameters.Add("ID", SqlDbType.Int).Value = Convert.ToInt32(dashboardID);
                    InsertCommand.Parameters.Add("Dashboard", SqlDbType.VarBinary).Value = stream.ToArray();
                    InsertCommand.Connection = connection;
                    InsertCommand.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }
    }
}

