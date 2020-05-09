using LitInject;
using UnityEngine;

public interface IClass
{
}

public class ClassA : IClass
{
}

public class ClassB
{
    //需要注入的依赖需要声明为带有public set的属性，并且添加[Inject]
    [Inject]
    public IClass Class1 { get; set; }
}

public class ClassC : ClassB
{
    [Inject]
    public IClass Class2 { get; set; }
}

public class Demo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var a = new ClassA();
        //向容器添加依赖
        LitInjector.AddToContainer(typeof(IClass), a);
        
        var d1 = new ClassC();
        //注入全部依赖
        LitInjector.InjectDependency(d1, false);
        Debug.Log((d1.Class1 == null) +"  " + (d1.Class2 == null));
        
        var d2 = new ClassC();
        //仅注入当前类中声明的依赖
        LitInjector.InjectDependency(d2, true);
        Debug.Log((d2.Class1 == null) +"  " + (d2.Class2 == null));
    }

}
