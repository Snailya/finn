using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Collections;

namespace FINN.UNITTEST;

public class XFuncTest
{
    [Test]
    public void TestParse()
    {
        var processor = new Processor();
        var exp = processor.Parse("if(ZPosition > 5000, 0, -1E-1) * XLength * YLength");

        var parameters = new ParameterCollection
        {
            { "zposition", 5000 },
            { "xlength", 1 },
            { "ylength", 1 }
        };
        var result = (NumberValue)exp.Execute(parameters);
        Assert.That(result.Number, Is.EqualTo(-1E-1));
    }
}