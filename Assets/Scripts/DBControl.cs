using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;

public class DBControl : MonoBehaviour
{
    public static MySqlConnection conn;
    private static string host = "nas.necohost.co.kr";
    private static string db_id = "final";
    private static string db_pw = "Soldesk@802";
    private static string db_schema = "final";

    private static string strConn = $"Server={host};Uid={db_id};Pwd={db_pw};Database={db_schema};";

    private void Awake()
    {
        InitializeConnection();
    }

    private void InitializeConnection()
    {
        try
        {
            conn = new MySqlConnection(strConn);
            conn.Open();
            if (conn.Ping())
            {
                Debug.LogWarning("DB is Connected!");
            }
            else
            {
                Debug.LogError("DB can't be connected!");
            }
            conn.Close();
            DontDestroyOnLoad(this.gameObject);
        }
        catch (System.Exception e)
        {
            Debug.LogError("DB Error: " + e.ToString());
        }
    }

    public static bool OnCUD(string query)
    {
        try
        {
            conn.Open();
            using (var sqlCommand = new MySqlCommand(query, conn))
            {
                sqlCommand.ExecuteNonQuery();
            }
            conn.Close();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("DB Error: " + e.ToString());
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            return false;
        }
    }

    public static DataSet OnRead(string query, string table_name)
    {
        try
        {
            conn.Open();
            using (var cmd = new MySqlCommand(query, conn))
            {
                using (var sd = new MySqlDataAdapter(cmd))
                {
                    var ds = new DataSet();
                    sd.Fill(ds, table_name);
                    return ds;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("DB Error: " + e.ToString());
            return null;
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (conn != null && conn.State == ConnectionState.Open)
        {
            conn.Close();
        }
    }
}
