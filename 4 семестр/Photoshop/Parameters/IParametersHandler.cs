namespace MyPhotoshop
{
    public interface IParametersHandler<out TParams>
    {
        ParameterInfo[] GetDescription();
        TParams CreateParams(double[] values);
    }
}