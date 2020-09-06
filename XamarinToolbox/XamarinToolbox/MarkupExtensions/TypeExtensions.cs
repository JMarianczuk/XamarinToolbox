using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinToolbox.MarkupExtensions
{
    [ContentProperty(nameof(Value))]
    public abstract class PrimitiveTypeExtensionBase<TPrimitiveType> : IMarkupExtension<TPrimitiveType>
    {
        public TPrimitiveType Value { get; set; }
        public TPrimitiveType ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
    public class BoolExtension : PrimitiveTypeExtensionBase<bool> { }
    public class ByteExtension : PrimitiveTypeExtensionBase<byte> { }
    public class SByteExtension : PrimitiveTypeExtensionBase<sbyte> { }
    public class CharExtension : PrimitiveTypeExtensionBase<char> { }
    public class ShortExtension : PrimitiveTypeExtensionBase<short> { }
    public class UShortExtension : PrimitiveTypeExtensionBase<ushort> { }
    public class IntExtension : PrimitiveTypeExtensionBase<int> { }
    public class UIntExtension : PrimitiveTypeExtensionBase<uint> { }
    public class LongExtension : PrimitiveTypeExtensionBase<long> { }
    public class ULongExtension : PrimitiveTypeExtensionBase<ulong> { }
    public class FloatExtension : PrimitiveTypeExtensionBase<float> { }
    public class DoubleExtension : PrimitiveTypeExtensionBase<double> { }
    public class DecimalExtension : PrimitiveTypeExtensionBase<decimal> { }
}