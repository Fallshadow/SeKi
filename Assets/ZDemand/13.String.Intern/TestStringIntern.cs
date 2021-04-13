using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class TestStringIntern : MonoBehaviour
{
    private string fixedString1 = "223";
    private string fixedString2 = "223";

    public void PrintResult(string fixedString9)
    {
        string fixedString3 = "223";
        string fixedString4 = fixedString1;
        string string1 = "2";
        string string2 = "23";
        string fixedString5 = string1 + string2;
        string fixedString6 = String.Intern(fixedString5);
        MyTestString myTestString = new MyTestString();
        myTestString.TestString = "223";
        string fixedString7 = myTestString.TestString;
        myTestString.TestString = string1 + string2;
        string fixedString12 = myTestString.TestString;
        int intTest = 223;
        string fixedString8 = intTest.ToString();
        string fixedString10 = String.Intern(intTest.ToString());
        string fixedString11 = String.Intern(fixedString7);
        
        Debug.Log("fixedString1 与 fixedString2");
        Debug.Log(Object.ReferenceEquals(fixedString1,fixedString2));
        
        Debug.Log("fixedString1 与 fixedString3");
        Debug.Log(Object.ReferenceEquals(fixedString1,fixedString3));

        Debug.Log("fixedString1 与 fixedString4");
        Debug.Log(Object.ReferenceEquals(fixedString1,fixedString4));
        
        Debug.Log("fixedString1 与 fixedString5");
        Debug.Log(Object.ReferenceEquals(fixedString1,fixedString5));
        
        Debug.Log("fixedString1 与 fixedString6");
        Debug.Log(Object.ReferenceEquals(fixedString1,fixedString6));
        
        Debug.Log("fixedString1 与 fixedString7");
        Debug.Log(Object.ReferenceEquals(fixedString1,fixedString7));
        
        Debug.Log("fixedString1 与 fixedString8");
        Debug.Log(Object.ReferenceEquals(fixedString1,fixedString8));
        
        Debug.Log("fixedString1 与 fixedString9");
        Debug.Log(Object.ReferenceEquals(fixedString1,fixedString9));
        
        Debug.Log("fixedString1 与 fixedString10");
        Debug.Log(Object.ReferenceEquals(fixedString1,fixedString10));
        
        Debug.Log("fixedString1 与 fixedString11");
        Debug.Log(Object.ReferenceEquals(fixedString1,fixedString11));
        
        Debug.Log("fixedString1 与 fixedString12");
        Debug.Log(Object.ReferenceEquals(fixedString1,fixedString12));
    }
}

public class MyTestString
{
    public string TestString = "TestString";
}
