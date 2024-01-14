using OpenTK.Mathematics;

namespace RockEngine.Common.Utils
{
    public static class Matrix3Extensions
    {
        public static Vector3 GetRow(this Matrix3 mat, int row)
        {
            if(row == 0)
            {
                return mat.Row0;
            }
            else if (row == 1)
            {
                return mat.Row1;
            }
            else if (row == 2)
            {
                return mat.Row2;
            }
            throw new InvalidOperationException("row can not be < 0 and > 2");
        }

        public static Vector3 GetColumn(this Matrix3 mat, int column)
        {
            if(column == 0)
            {
                return mat.Column0;
            }
            else if(column == 1)
            {
                return mat.Column1;
            }
            else if(column == 2)
            {
                return mat.Column2;
            }
            throw new InvalidOperationException("column can not be < 0 and > 2");
        }
    }
}
