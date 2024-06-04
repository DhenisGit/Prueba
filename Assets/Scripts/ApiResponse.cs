using System.Collections.Generic;

[System.Serializable]
public class ApiResponse
{
    public List<DataItem> data;
    public int size;
}

[System.Serializable]
public class DataItem
{
    public int id;
    public int maquina_id;
    public int user_id;
    public string estado_registro;
    public Maquina maquina;
}

[System.Serializable]
public class Maquina
{
    public int id;
    public string nombre;
    public string url_imagen;
}
