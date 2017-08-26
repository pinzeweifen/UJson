
using System.Collections.Generic;

public class JsonArray
{
    private List<JsonInfo> list = new List<JsonInfo>();

    public JsonArray() {
    }

    public JsonArray(List<JsonInfo> list)
    {
        this.list = list;
    }

    public int count()
    {
        if (list != null)
            return list.Count;
        return 0;
    }

    public void add(JsonObject value)
    {
        if (list == null) return;

        list.Add(value.getJsonInfo());
    }
    
    public void add(JsonValue value)
    {
        if (list == null) return;

        list.Add(value.toJsonInfo());
    }

    public void insert(int index, JsonValue value)
    {
        if (list == null) return;

        list.Insert(index, value.toJsonInfo());
    }

    public void addList(List<JsonValue> list)
    {
        if (list == null) return;

        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            this.list.Add(list[i].toJsonInfo());
        }
    }

    public void setItem(int index, JsonValue value)
    {
        this.list[index] = value.toJsonInfo();
    }

    public JsonValue at(int i)
    {
        if (list == null) return null;

        return new JsonValue(list[i]);
    }

    public int indexOf(JsonValue value)
    {
        int end = list.Count;
        for (int i = 0; i < end; i++)
        {
            if (list[i] == value.toJsonInfo())
            {
                return i;
            }
        }
        return -1;
    }

    public List<JsonValue> all()
    {
        if (list == null) return null;

        List<JsonValue> valueList = new List<JsonValue>();
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            valueList.Add(new JsonValue(list[i]));
        }

        return valueList;
    }

    public void removeAll()
    {
        if (list == null) return;

        list.Clear();
    }

    public void removeAt(int i)
    {
        if (list == null) return;

        list.RemoveAt(i);
    }

    public JsonValue takeAt(int i)
    {
        if (list == null) return null;

        var value = new JsonValue(list[i]);
        list.RemoveAt(i);
        return value;
    }
}
