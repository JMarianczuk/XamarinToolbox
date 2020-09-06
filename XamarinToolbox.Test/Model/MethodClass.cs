namespace XamarinToolbox.Test.Model
{
    public class MethodClass
    {
        public string NoParameter()
        {
            return "noParameter";
        }

        public string WithParameter(int parameter)
        {
            return "withParameter" + parameter;
        }

        public string DoubleParameter(double parameter)
        {
            return "doubleParameter" + parameter;
        }

        public string FloatParameter(float parameter)
        {
            return "floatParameter" + parameter;
        }

        public string ByteParameter(byte parameter)
        {
            return "byteParameter" + parameter;
        }

        public string BoolParameter(bool parameter)
        {
            return "boolParameter" + parameter;
        }

        public string CharParameter(char parameter)
        {
            return "charParameter" + parameter + "after";
        }
    }
}