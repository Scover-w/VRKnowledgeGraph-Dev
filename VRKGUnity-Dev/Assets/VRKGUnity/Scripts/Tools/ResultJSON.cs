using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultJson
{
    public Head Head;
    public Results Results;
}

public class Head
{
    public List<string> Vars;
}

public class Results
{
    public Bindings Bindings;
}

public class Bindings
{
    public List<Binding> Binding;
}

public class Binding
{

}