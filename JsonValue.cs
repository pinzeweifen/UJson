
using System.Collections.Generic;

public class JsonValue
{
    private JsonInfo jsonInfo;

    public JsonValue() { }

    public JsonValue(string key)
    {
        setValue(key);
        jsonInfo.type = ValueType.Null;
    }

    public JsonValue(JsonInfo jsonInfo)
    {
        this.jsonInfo = jsonInfo;
    }

    public JsonValue(string key, string value)
    {
        setkey(key);
        setValue(value);
    }

    public JsonValue(string key, bool value)
    {
        setkey(key);
        setValue(value);
    }

    public JsonValue(string key, int value)
    {
        setkey(key);
        setValue(value);
    }

    public JsonValue(string key, long value)
    {
        setkey(key);
        setValue(value);
    }

    public JsonValue(string key, float value)
    {
        setkey(key);
        setValue(value);
    }

    public JsonValue(string key, double value)
    {
        setkey(key);
        setValue(value);
    }

    public void setkey(string key)
    {
        jsonInfo.key = key;
    }

    public void setValue()
    {
        setValue(string.Empty, ValueType.Null);
    }

    public void setValue(string value)
    {
        setValue('"' + value + '"', ValueType.String);
    }

    public void setValue(bool value)
    {
        setValue(value.ToString(), ValueType.Logic);
    }

    public void setValue(int value)
    {
        setValue(value.ToString(), ValueType.Number);
    }
    public void setValue(long value)
    {
        setValue(value.ToString(), ValueType.Number);
    }
    public void setValue(float value)
    {
        setValue(value.ToString(), ValueType.Number);
    }
    public void setValue(double value)
    {
        setValue(value.ToString(), ValueType.Number);
    }

    public void setValue(JsonArray value)
    {
        jsonInfo.type = ValueType.Array;
        var list = value.all();
        int count = list.Count;

        jsonInfo.list = new List<JsonInfo>();
        for (int i = 0; i < count; i++)
        {
            jsonInfo.list.Add(list[i].toJsonInfo());
        }
    }

    public void setValue(JsonObject value)
    {
        jsonInfo.type = ValueType.Object;
        jsonInfo.list = new List<JsonInfo>();
        jsonInfo.list.Add(value.getJsonInfo());
    }

    public bool isBool()
    {
        return jsonInfo.type == ValueType.Logic;
    }

    public bool isArray()
    {
        return jsonInfo.type == ValueType.Array;
    }

    public bool isObject()
    {
        return jsonInfo.type == ValueType.Object;
    }

    public bool isfloat()
    {
        float num;
        return float.TryParse(jsonInfo.value, out num);
    }

    public bool isDouble()
    {
        double num;
        return double.TryParse(jsonInfo.value, out num);
    }

    public bool isInt()
    {
        int num;
        return int.TryParse(jsonInfo.value, out num);
    }

    public bool isLong()
    {
        long num;
        return long.TryParse(jsonInfo.value, out num);
    }

    public bool isNull()
    {
        return jsonInfo.type == ValueType.Null;
    }

    public bool isString()
    {
        return jsonInfo.type == ValueType.String;
    }

    public string toKey()
    {
        return jsonInfo.key;
    }

    public bool toBool()
    {
        return bool.Parse(jsonInfo.value);
    }

    public JsonArray toArray()
    {
        return new JsonArray(jsonInfo.list);
    }

    public JsonObject toObject()
    {
        return new JsonObject(jsonInfo);
    }

    public float toFloat()
    {
        return float.Parse(jsonInfo.value);
    }

    public double toDouble()
    {
        return double.Parse(jsonInfo.value);
    }

    public int toint()
    {
        return int.Parse(jsonInfo.value);
    }

    public long toLong()
    {
        return long.Parse(jsonInfo.value);
    }

    public string toString()
    {
        return jsonInfo.value;
    }

    public JsonInfo toJsonInfo()
    {
        return jsonInfo;
    }

    private void setValue(string value, ValueType type)
    {
        jsonInfo.value = value;
        jsonInfo.type = ValueType.Number;
    }

}
