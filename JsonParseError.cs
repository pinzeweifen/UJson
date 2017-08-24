

public enum ParseError
{
    //未发生错误
    NoError = 0,
    //对象不正确地终止以右花括号结束
    UnterminatedObject,
    //分隔不同项的逗号丢失
    MissingNameSeparator,
    //数组不正确地终止以右中括号结束
    UnterminatedArray,
    //对象中分割 key/value 的冒号丢失
    MissingValueSeparator,
    //值是非法的
    IllegalValue,
    //在解析数字时，输入流结束
    TerminationByNumber,
    //数字格式不正确
    IllegalNumber,
    //在输入时，发生一个非法转义序列
    IllegalEscapeSequence,
    //字符串不是以引号结束
    UnterminatedString,
    //解析的文档在末尾处包含额外的乱码
    GarbageAtEnd
}

public struct JsonParseError
{
    public ParseError error;
}

