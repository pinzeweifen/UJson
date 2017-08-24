
public enum TokenType
{
    START_OBJ, END_OBJ, START_ARRAY, END_ARRAY, NULL, NUMBER, STRING, BOOLEAN, COLON, COMMA, END_DOC
}

public class Token
{
    private TokenType type;
    private string value;

    public Token(TokenType type, string value)
    {
        this.type = type;
        this.value = value;
    }

    public TokenType getType()
    {
        return this.type;
    }

    public string getValue()
    {
        return value;
    }
}
