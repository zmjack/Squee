using ExcelSharp.NPOI;
using Richx;

namespace Squee.Antd;

public static class ExcelSheetExtensions
{
    public static T Fetch<T>(this ExcelSheet @this, Cursor cursor) where T : IUploadModel<T>
    {
        return @this.Fetch<T>(cursor);
    }
}
