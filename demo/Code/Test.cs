using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    private void Awake()
    {
        var json = FileOperate.ReadFileToString("arpg/Code/Json/demo/txt/equip.json");

        JsonParseError jsonError = new JsonParseError();
        JsonDocument document = JsonDocument.fromJson(json, ref jsonError);

        if(jsonError.error == ParseError.NoError)
        {
            if (document.isObject())
            {
                var o = document.toObject();
                foreach(var item in o.keys())
                {
                    var v = o.value(item);
                    if (v.isArray())
                    {
                        var a = v.toArray();
                        foreach (var t in a.all())
                        {
                            if (t.isObject())
                            {
                                Debug.Log(t.toObject().value("name").toString());
                            }
                        }
                    }
                }
            }
        }
    }
}
