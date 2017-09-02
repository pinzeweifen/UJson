
using System.Collections.Generic;

public enum ValueType
{
    Array, Object, Null, Number, Logic, String
}

public class JsonInfo
{
    public string key;
    public string value;
    public ValueType type;
    public List<JsonInfo> list;
}

public class JsonDocument
{
    private JsonInfo jsonInfo = new JsonInfo();

    public static JsonDocument fromJson(string json, ref JsonParseError jsonError)
    {
        JsonDocument document = new JsonDocument();
        Tokenizer tokenizer = new Tokenizer(json);

        tokenizer.Tokenize(ref jsonError);
        if (jsonError.error != ParseError.NoError)
            return null;

        //检查右括号结尾是否错误
        if (-1 == GetValueEndIndex(tokenizer.Tokens, 0))
        {
            jsonError.error = ParseError.UnterminatedObject;
            return null;
        }

        //检查逗号
        if (isSeparate(tokenizer.Tokens, ref jsonError))
        {
            jsonError.error = ParseError.MissingNameSeparator;
            return null;
        }

        document.jsonInfo = JObject(tokenizer.Tokens, 0, ref jsonError);

        return document;
    }

    public void setJsonObject(JsonObject json)
    {
        this.jsonInfo = json.getJsonInfo();
    }
    
    public void setJsonInfo(JsonInfo jsonInfo)
    {
        this.jsonInfo = jsonInfo;
    }

    public string toJson()
    {
        var str = toJson(jsonInfo);
        return str.Remove(str.Length - 1);
    }
    
    public bool isArray()
    {
        return jsonInfo.type == ValueType.Array;
    }

    public bool isObject()
    {
        return jsonInfo.type == ValueType.Object;
    }

    public bool isNull()
    {
        return jsonInfo.type == ValueType.Null;
    }

    public JsonObject toObject()
    {
        return new JsonObject(this.jsonInfo);
    }

    public JsonArray toArray()
    {
        return new JsonArray(this.jsonInfo.list);
    }

    private string toJson(JsonInfo Info)
    {
        string tmp = string.Empty;
        if (Info.key != null)
            tmp += '"' + Info.key + "\":";

        if (Info.type == ValueType.Array || Info.type == ValueType.Object)
        {
            if (Info.list != null)
            {
                tmp += Info.type == ValueType.Array ? "[" : "{";
                int count = Info.list.Count;
                for (int i = 0; i < count; i++)
                {
                    tmp += toJson(Info.list[i]);
                }

                tmp = removeComma(tmp);
                tmp += Info.type == ValueType.Array ? "]," : "},";
            }
        }
        else
        {
            tmp += value(Info.type, Info.value);
        }

        return tmp;
    }

    private string value(ValueType type, string str)
    {
        if (type == ValueType.String)
            return str + "\",";
        else
            return str + ",";
    }

    private string removeComma(string str)
    {
        if (str[str.Length - 1] == ',')
            return str.Remove(str.Length - 1);
        return str;
    }

    /// <summary>
    /// tokens[index].getType() == TokenType.START_OBJ
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static JsonInfo JObject(List<Token> tokens, int index, ref JsonParseError jsonError)
    {
        JsonInfo info = GetJsonInfo(tokens, index, ref jsonError);
        if (info == null) return null;
        info.type = ValueType.Object;

        info.list = GetAttribute(tokens, index, ref jsonError);

        return info;
    }

    /// <summary>
    /// tokens[index].getType() == TokenType.START_ARRAY
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static JsonInfo JArray(List<Token> tokens, int index, ref JsonParseError jsonError)
    {
        JsonInfo info = GetJsonInfo(tokens, index, ref jsonError);
        if (info == null) return null;
        info.type = ValueType.Array;
        info.list = new List<JsonInfo>();
        int end = GetValueEndIndex(tokens, index);

        switch (tokens[index + 1].getType())
        {
            case TokenType.START_OBJ:
                for (int i = index + 1; i < end; i += 2)
                {
                    info.list.Add(JObject(tokens, i, ref jsonError));
                    i = GetValueEndIndex(tokens, i);
                }
                break;
            case TokenType.START_ARRAY:
                for (int i = index + 1; i < end; i += 2)
                {
                    info.list.Add(JArray(tokens, i, ref jsonError));
                    i = GetValueEndIndex(tokens, i);
                }
                break;
            default:
                for (int i = index + 1; i < end; i += 2)
                {
                    JsonInfo ji = new JsonInfo
                    {
                        value = tokens[i].getValue(),
                        type = GetType(tokens, i)
                    };
                    info.list.Add(ji);
                }
                break;
        }

        return info;
    }

    private static JsonInfo JValue(List<Token> tokens, int index, ref JsonParseError jsonError)
    {
        JsonInfo info = new JsonInfo();
        info.type = GetType(tokens, index);
        info.key = GetKey(tokens, index, ref jsonError);
        info.value = tokens[index].getValue();

        return info;
    }

    /// <summary>
    /// object -> attributes
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static List<JsonInfo> GetAttribute(List<Token> tokens, int index, ref JsonParseError jsonError)
    {
        List<JsonInfo> list = new List<JsonInfo>();
        int end = GetValueEndIndex(tokens, index);

        for (int i = index + 1; i < end; i++)
        {
            if (tokens[i].getType() == TokenType.COLON)
            {
                i++;
                switch (tokens[i].getType())
                {
                    case TokenType.START_ARRAY:
                        list.Add(JArray(tokens, i, ref jsonError));
                        i = GetValueEndIndex(tokens, i);
                        break;
                    case TokenType.START_OBJ:
                        list.Add(JObject(tokens, i, ref jsonError));
                        i = GetValueEndIndex(tokens, i);
                        break;
                    case TokenType.BOOLEAN:
                    case TokenType.NUMBER:
                    case TokenType.NULL:
                    case TokenType.STRING:
                        list.Add(JValue(tokens, i, ref jsonError));
                        break;
                }
            }
        }

        return list;
    }

    private static ValueType GetType(List<Token> tokens, int index)
    {
        switch (tokens[index].getType())
        {
            case TokenType.STRING:
                return ValueType.String;
            case TokenType.NUMBER:
                return ValueType.Number;
            case TokenType.BOOLEAN:
                return ValueType.Logic;
            case TokenType.START_ARRAY:
                return ValueType.Array;
            case TokenType.START_OBJ:
                return ValueType.Object;
        }
        return ValueType.Null;
    }

    private static int GetValueEndIndex(List<Token> tokens, int index)
    {
        TokenType endType = TokenType.END_OBJ;
        int count = tokens.Count;
        TokenType startType = tokens[index].getType();
        Stack<TokenType> stack = new Stack<TokenType>();

        if (startType == TokenType.START_OBJ)
            endType = TokenType.END_OBJ;
        else if (startType == TokenType.START_ARRAY)
            endType = TokenType.END_ARRAY;
        else
            return -1;

        for (int i = index; i < count; i++)
        {
            if (startType == tokens[i].getType())
            {
                stack.Push(startType);
            }
            else if (endType == tokens[i].getType())
            {
                if (startType == stack.Peek())
                {
                    stack.Pop();
                    if (stack.Count <= 0)
                        return i;
                }
            }
        }

        return -1;
    }

    /*
    private static int GetItemCount(List<Token> tokens, int index)
    {
        int count = 0;
        int end = GetValueEndIndex(tokens, index);
        for (int i = index+1; i < end; i+=2)
        {
            if (i != -1)
            {
                i = GetValueEndIndex(tokens, i);
            }
            else
            {
               i = GetCommaindex(tokens, i, end);
            }
            count++;
        }
        return count;
    }*/

    private static int GetCommaindex(List<Token> tokens, int index, int end)
    {
        for (int i = index; i < end; i++)
        {
            if (tokens[i].getType() == TokenType.COMMA)
                return i;
        }

        return -1;
    }

    private static JsonInfo GetJsonInfo(List<Token> tokens, int index, ref JsonParseError jsonError)
    {
        JsonInfo info = new JsonInfo();
        info.key = GetKey(tokens, index, ref jsonError);
        int end = GetValueEndIndex(tokens, index);
        if (end == -1) return null;
        info.value = GetValue(tokens, index, end);

        return info;
    }

    private static string GetValue(List<Token> tokens, int start, int end)
    {
        string value = string.Empty;
        for (int i = start; i <= end; i++)
        {
            if (tokens[i].getType() == TokenType.STRING)
                value += GetString(tokens[i]);
            else
                value += tokens[i].getValue();
        }
        return value;
    }

    private static string GetString(Token token)
    {
        if (token.getValue() == "")
            return "";
        return '"' + token.getValue() + '"';
    }

    private static string GetKey(List<Token> tokens, int index, ref JsonParseError jsonError)
    {
        if (index == 0) return string.Empty;

        if (tokens[index - 1].getType() == TokenType.COLON)
            return tokens[index - 2].getValue();

        if (tokens[index - 1].getType() == TokenType.START_ARRAY || tokens[index - 1].getType() == TokenType.COMMA)
            return string.Empty;

        jsonError.error = ParseError.MissingValueSeparator;
        return string.Empty;
    }

    private static bool isSeparate(List<Token> tokens, ref JsonParseError jsonError)
    {
        int count = tokens.Count;
        for (int i = 0; i < count; i++)
        {
            switch (tokens[i].getType())
            {
                case TokenType.END_ARRAY:
                    if (tokens[i + 1].getType() == TokenType.END_OBJ)
                        break;
                    if (tokens[i + 1].getType() != TokenType.COMMA)
                        return true;
                    break;

                case TokenType.END_OBJ:
                    if (tokens[i + 1].getType() == TokenType.END_ARRAY || tokens[i + 1].getType() == TokenType.END_DOC)
                        break;
                    if (tokens[i + 1].getType() != TokenType.COMMA)
                        return true;
                    break;

                case TokenType.NULL:
                case TokenType.NUMBER:
                case TokenType.BOOLEAN:
                    if (tokens[i + 1].getType() == TokenType.END_ARRAY || tokens[i + 1].getType() == TokenType.END_OBJ)
                        break;
                    if (tokens[i + 1].getType() != TokenType.COMMA)
                        return true;
                    break;

                case TokenType.STRING:
                    if (tokens[i - 1].getType() == TokenType.COLON)
                    {
                        if (tokens[i + 1].getType() == TokenType.END_ARRAY || tokens[i + 1].getType() == TokenType.END_OBJ)
                            break;
                        if (tokens[i + 1].getType() != TokenType.COMMA)
                            return true;
                    }
                    break;
            }
        }
        return false;
    }

}

