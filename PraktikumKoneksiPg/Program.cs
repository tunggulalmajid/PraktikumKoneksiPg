using Npgsql;

public class Database
{
    private string _connString;

    public Database(string connString)
    {
        _connString = connString;
    }

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connString);
    }
}

public static class Config
{
    public static string connString =
          "Host=localhost;" +  // server PostgreSQL
          "Port=5432;" +  // port default PostgreSQL
          "Database=db_mahasiswa;" +
          "Username=postgres;" +
          "Password=admin123";
}

public class Mahasiswa
{
    private Database db;
    public Mahasiswa()
    {
        db = new Database(Config.connString);
    }

    public void TambahMahasiswa(string nama, string nim, string jurusan, decimal ipk)
    {
        string sql = @"INSERT INTO mahasiswa (nama, nim, jurusan, ipk) VALUES (@nama, @nim, @jurusan, @ipk)";
        //string sql = $"INSERT INTO mahasiswa (nama, nim, jurusan, ipk) VALUES ({nama}, {nim}, {jurusan}, {ipk})";

        try
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    // Parameter binding — AMAN dari SQL Injection!
                    cmd.Parameters.AddWithValue("@nama", nama);
                    cmd.Parameters.AddWithValue("@nim", nim);
                    cmd.Parameters.AddWithValue("@jurusan", jurusan);
                    cmd.Parameters.AddWithValue("@ipk", ipk);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"{rowsAffected} data berhasil ditambahkan.");
                }
            }
        }
        catch (Exception ex) 
        {
            Console.WriteLine(ex.Message);
        }
        
    }

    public List<string> GetSemuaMahasiswa()
    {
        var hasil = new List<string>();
        string sql = "SELECT id, nama, nim, jurusan, ipk FROM mahasiswa";

        using (var conn = db.GetConnection())
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string nama = reader.GetString(1);
                        string nim = reader.GetString(2);
                        string jurusan = reader.GetString(3);
                        decimal ipk = reader.GetDecimal(4);
                        hasil.Add($"[{id}] {nama} - {nim} - {jurusan} - IPK:{ipk}");
                    }
                }
            }
           
        }
        return hasil;
    }

    public void UpdateMahasiswa(int id, string jurusanBaru, decimal ipkBaru)
    {
        string sql = @"UPDATE mahasiswa 
            SET jurusan = @jurusan, ipk = @ipk 
            WHERE id = @id";

        using (var conn = db.GetConnection())
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@jurusan", jurusanBaru);
                cmd.Parameters.AddWithValue("@ipk", ipkBaru);

                int affected = cmd.ExecuteNonQuery();

                if (affected > 0)
                    Console.WriteLine("Data berhasil diupdate!");
                else
                    Console.WriteLine("ID tidak ditemukan.");
            }
        }
    }

    public void HapusMahasiswa(int id)
    {
        string sql = "DELETE FROM mahasiswa WHERE id = @id";

        using (var conn = db.GetConnection())
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                int affected = cmd.ExecuteNonQuery();

                if (affected > 0)
                    Console.WriteLine($"Mahasiswa ID {id} dihapus.");
                else
                    Console.WriteLine("Data tidak ditemukan.");
            }
        }
    }
}


internal class Program
{
    private static void Main(string[] args)
    {
        Mahasiswa mhs = new Mahasiswa();
        //mhs.TambahMahasiswa("Subar7", "242410102027", "Teknologi Informasi", 4);
        //List<string> listMahasiswa = mhs.GetSemuaMahasiswa();
        //foreach (var value in listMahasiswa) 
        //{
        //    Console.WriteLine(value);
        //}
        //mhs.UpdateMahasiswa(3, "Sistem Informasi", 3);
        mhs.UpdateMahasiswa(3, "Sistem Informasi", 3);
        mhs.HapusMahasiswa(10);
    }
}