using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
public class TransactionLog
{
    public string Timestamp { get; set; }
    public string Usuario { get; set; }
    public string Operacion { get; set; }
    public string Detalles { get; set; }

    public TransactionLog(string timestamp, string usuario, string operacion, string detalles)
    {
        Timestamp = timestamp;
        Usuario = usuario;
        Operacion = operacion;
        Detalles = detalles;
    }
}
