
using System.Collections.Generic;
using System.Text;

public class Tokenizer
{
    private int index = 0;
    private string data;
    private bool isUnread = false;
    private int savedChar;
    private int c;
    List<Token> tokens = new List<Token>();

    public Tokenizer(string data)
    {
        this.data = data;
    }

    public List<Token> Tokens
    {
        get { return tokens; }
    }

    public void Tokenize(ref JsonParseError jsonError)
    {
        jsonError.error = ParseError.NoError;
        Token token;
        do
        {
            if ((token = Inspect(ref jsonError)) == null ||
                jsonError.error != ParseError.NoError)
                break;
            tokens.Add(token);
        } while (token.getType() != TokenType.END_DOC);

        if (tokens[tokens.Count - 2].getType() != TokenType.END_OBJ)
            jsonError.error = ParseError.GarbageAtEnd;
    }

    private Token Inspect(ref JsonParseError jsonError)
    {
        c = '?';
        while (isSpace(c = read())) ;
        if (isNull(c, ref jsonError))
        {
            return new Token(TokenType.NULL, null);
        }
        else if (c == ',')
        {
            return new Token(TokenType.COMMA, ",");
        }
        else if (c == ':')
        {
            return new Token(TokenType.COLON, ":");
        }
        else if (c == '{')
        {
            return new Token(TokenType.START_OBJ, "{");
        }
        else if (c == '[')
        {
            return new Token(TokenType.START_ARRAY, "[");
        }
        else if (c == ']')
        {
            return new Token(TokenType.END_ARRAY, "]");
        }
        else if (c == '}')
        {
            return new Token(TokenType.END_OBJ, "}");
        }
        else if (isTrue(c, ref jsonError))
        {
            return new Token(TokenType.BOOLEAN, "true");
        }
        else if (isFalse(c, ref jsonError))
        {
            return new Token(TokenType.BOOLEAN, "false");
        }
        else if (c == '"')
        {
            return readString(ref jsonError);
        }
        else if (isNum(c))
        {
            unread();
            return readNum(ref jsonError);
        }else if(c == -1)
        {
            return new Token(TokenType.END_DOC, "EOF");
        }

        return null;
    }

    private int read()
    {
        if (index >= data.Length) return -1;

        if (!isUnread)
        {
            int c = data[index++];
            savedChar = c;
            return c;
        }
        else
        {
            isUnread = false;
            return savedChar;
        }
    }

    private void unread()
    {
        isUnread = true;
    }

    private bool isSpace(int c)
    {
        return c >= 0 && c <= ' ';
    }

    private bool isTrue(int c, ref JsonParseError jsonError)
    {
        if (c == 't')
        {
            if (read() == 'r')
            {
                if (read() == 'u')
                {
                    if (read() == 'e')
                    {
                        return true;
                    }
                    jsonError.error = ParseError.IllegalValue;
                }
                else
                {
                    jsonError.error = ParseError.IllegalValue;
                }
            }
            else
            {
                jsonError.error = ParseError.IllegalValue;
            }
        }
        return false;
    }

    private bool isFalse(int c, ref JsonParseError jsonError)
    {
        if (c == 'f')
        {
            if (read() == 'a')
            {
                if (read() == 'l')
                {
                    if (read() == 's')
                    {
                        if (read() == 'e')
                        {
                            return true;
                        }
                        jsonError.error = ParseError.IllegalValue;
                    }
                    else
                    {
                        jsonError.error = ParseError.IllegalValue;
                    }
                }
                else
                {
                    jsonError.error = ParseError.IllegalValue;
                }
            }
            else
            {
                jsonError.error = ParseError.IllegalValue;
            }
        }
        return false;
    }

    private bool isEscape(ref JsonParseError jsonError)
    {
        if (c == '\\')
        {
            c = read();
            if (c == '"' || c == '\\' || c == '/' || c == 'b' ||
                    c == 'f' || c == 'n' || c == 't' || c == 'r' || c == 'u')
            {
                return true;
            }
            else
            {
                jsonError.error = ParseError.IllegalEscapeSequence;
            }
        }
        return false;
    }

    private bool isNull(int c, ref JsonParseError jsonError)
    {
        if (c == 'n')
        {
            if (read() == 'u')
            {
                if (read() == 'l')
                {
                    if (read() == 'l')
                    {
                        return true;
                    }
                    jsonError.error = ParseError.IllegalValue;
                }
                else
                {
                    jsonError.error = ParseError.IllegalValue;
                }
            }
            else
            {
                jsonError.error = ParseError.IllegalValue;
            }
        }
        return false;
    }

    private bool isDigit(int c)
    {
        return c >= '0' && c <= '9';
    }

    private bool isDigitOne2Nine(int c)
    {
        return c >= '1' && c <= '9';
    }

    private bool isSep(int c)
    {
        return c == '}' || c == ']' || c == ',';
    }

    private bool isNum(int c)
    {
        return isDigit(c) || c == '-';
    }

    private Token readString(ref JsonParseError jsonError)
    {
        StringBuilder sb = new StringBuilder();
        while (true)
        {
            c = read();
            if (isEscape(ref jsonError))
            {    //判断是否为\", \\, \/, \b, \f, \n, \t, \r.
                if (c == 'u')
                {
                    sb.Append('\\' + (char)c);
                    for (int i = 0; i < 4; i++)
                    {
                        c = read();
                        if (isHex(c))
                        {
                            sb.Append((char)c);
                        }
                        else
                        {
                            jsonError.error = ParseError.IllegalNumber;
                            return null;
                        }
                    }
                }
                else
                {
                    sb.Append("\\" + (char)c);
                }
            }
            else if (c == '"')
            {
                return new Token(TokenType.STRING, sb.ToString());
            }
            else if (c == '\r' || c == '\n')
            {
                jsonError.error = ParseError.UnterminatedString;
                return null;
            }
            else
            {
                sb.Append((char)c);
            }
        }
    }

    private Token readNum(ref JsonParseError jsonError)
    {
        StringBuilder sb = new StringBuilder();
        int c = read();
        if (c == '-')
        { //-
            sb.Append((char)c);
            c = read();
            if (c == '0')
            { //-0
                sb.Append((char)c);
                numAppend(sb, ref jsonError);

            }
            else if (isDigitOne2Nine(c))
            { //-digit1-9
                do
                {
                    sb.Append((char)c);
                    c = read();
                } while (isDigit(c));
                unread();
                numAppend(sb, ref jsonError);
            }
            else
            {
                jsonError.error = ParseError.IllegalNumber;
                return null;
            }
        }
        else if (c == '0')
        { //0
            sb.Append((char)c);
            numAppend(sb, ref jsonError);
        }
        else if (isDigitOne2Nine(c))
        { //digit1-9
            do
            {
                sb.Append((char)c);
                c = read();
            } while (isDigit(c));
            unread();
            numAppend(sb, ref jsonError);
        }
        return new Token(TokenType.NUMBER, sb.ToString()); //the value of 0 is null
    }

    private void AppendFrac(StringBuilder sb)
    {
        c = read();
        while (isDigit(c))
        {
            sb.Append((char)c);
            c = read();
        }
    }

    private void AppendExp(StringBuilder sb, ref JsonParseError jsonError)
    {
        int c = read();
        if (c == '+' || c == '-')
        {
            sb.Append((char)c); //Append '+' or '-'
            c = read();
            if (!isDigit(c))
            {
                jsonError.error = ParseError.IllegalNumber;
                return;
            }
            else
            { //e+(-) digit
                do
                {
                    sb.Append((char)c);
                    c = read();
                } while (isDigit(c));
                unread();
            }
        }
        else if (!isDigit(c))
        {
            jsonError.error = ParseError.IllegalNumber;
            return;
        }
        else
        { //e digit
            do
            {
                sb.Append((char)c);
                c = read();
            } while (isDigit(c));
            unread();
        }
    }

    private void numAppend(StringBuilder sb, ref JsonParseError jsonError)
    {
        c = read();
        if (c == '.')
        { //int frac
            sb.Append((char)c); //apppend '.'
            AppendFrac(sb);
            if (isExp(c))
            { //int frac exp
                sb.Append((char)c); //Append 'e' or 'E';
                AppendExp(sb, ref jsonError);
            }

        }
        else if (isExp(c))
        { // int exp
            sb.Append((char)c); //Append 'e' or 'E'
            AppendExp(sb, ref jsonError);
        }
        else
        {
            unread();
        }
    }

    private bool isHex(int c)
    {
        return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') ||
                (c >= 'A' && c <= 'F');
    }

    private bool isExp(int c)
    {
        return c == 'e' || c == 'E';
    }

    private Token next()
    {
        Token token = tokens[0];
        tokens.RemoveAt(0);
        return token;
    }

    private Token peek(int i)
    {
        return tokens[i];
    }

    private bool hasNext()
    {
        return tokens[0].getType() != TokenType.END_DOC;
    }

}
