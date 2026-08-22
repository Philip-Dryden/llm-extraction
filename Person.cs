class Person {

    public required string Name {get; set;}
    public int? Age {get; set;}
    public required Dictionary<string, List<Need>> Needs {get; set;}
    public required List<ClientTask> Tasks {get; set;} 

}