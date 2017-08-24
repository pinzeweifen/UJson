using System.Collections.Generic;

public class JsonObject
{
    private JsonInfo jsonInfo;
    public JsonObject(JsonInfo jsonInfo)
    {
        this.jsonInfo = jsonInfo;
    }

    public bool contains(string key)
    {
        if (getKey(key) != null)
            return true;

        return false;
    }

    public JsonInfo getJsonInfo()
    {
        return jsonInfo;
    }

    public JsonValue take(string key)
    {
        return new JsonValue(removeKey(key));
    }

    public JsonValue value(string key)
    {
        if (!isNull())
        {
            int count = jsonInfo.list.Count;
            for (int i = 0; i < count; i++)
            {
                if (jsonInfo.list[i].key == key)
                {
                    JsonValue value = new JsonValue(jsonInfo.list[i]);
                    return value;
                }
            }
        }
        return null;
    }

    public List<JsonValue> valueAll()
    {
        if (isNull()) return null;

        List<JsonValue> list = new List<JsonValue>();
        int count = jsonInfo.list.Count;
        for (int i = 0; i < count; i++)
        {
            list.Add(new JsonValue(jsonInfo.list[i]));
        }
        return list;
    }

    public void add(JsonValue value)
    {
        jsonInfo.list.Add(value.toJsonInfo());
    }

    public void insert(int index, JsonValue value)
    {
        jsonInfo.list.Insert(index, value.toJsonInfo());
    }

    public void setValue(int index, JsonValue value)
    {
        jsonInfo.list[index] = value.toJsonInfo();
    }

    public void remove(string key)
    {
        removeKey(key);
    }

    public void removeAll()
    {
        if (isNull())
        {
            jsonInfo.list.Clear();
        }
    }

    public int count()
    {
        if (!isNull())
        {
            return jsonInfo.list.Count;
        }
        return 0;
    }

    public List<string> keys()
    {
        List<string> list = new List<string>();

        if (!isNull())
        {
            int count = jsonInfo.list.Count;
            for (int i = 0; i < count; i++)
            {
                list.Add(jsonInfo.list[i].key);
            }
        }
        return list;
    }

    private JsonInfo removeKey(string key)
    {
        JsonInfo info = null;
        if (!isNull())
        {
            int count = jsonInfo.list.Count;
            for (int i = 0; i < count; i++)
            {
                if (jsonInfo.list[i].key == key)
                {
                    info = jsonInfo.list[i];
                    jsonInfo.list.Remove(jsonInfo.list[i]);
                    break;
                }
            }
        }
        return info;
    }

    private JsonInfo getKey(string key)
    {
        if (!isNull())
        {
            int count = jsonInfo.list.Count;
            for (int i = 0; i < count; i++)
            {
                if (jsonInfo.list[i].key == key)
                    return jsonInfo.list[i];
            }
        }
        return null;
    }

    private bool isNull()
    {
        return jsonInfo.list == null;
    }
}
