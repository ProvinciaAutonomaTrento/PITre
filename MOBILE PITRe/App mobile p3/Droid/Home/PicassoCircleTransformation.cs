using System;
using Android.Content;
using Android.Graphics;
using Com.Bumptech.Glide.Load;
using Com.Bumptech.Glide.Load.Engine;
using Java.Security;

namespace InformaticaTrentinaPCL.Droid.Home
{
    public class PicassoCircleTransformation : Java.Lang.Object, ITransformation
    {
        public string Key => "circle";

        public Bitmap Transform(Bitmap source)
        {
            int size = Math.Min(source.Width, source.Height);

			int x = (source.Width - size) / 2;
			int y = (source.Height - size) / 2;

			Bitmap squaredBitmap = Bitmap.CreateBitmap(source, x, y, size, size);
			if (squaredBitmap != source)
			{
				source.Recycle();
			}

            Bitmap bitmap = Bitmap.CreateBitmap(size, size, source.GetConfig());

			Canvas canvas = new Canvas(bitmap);
			Paint paint = new Paint();
			BitmapShader shader = new BitmapShader(squaredBitmap,BitmapShader.TileMode.Clamp, BitmapShader.TileMode.Clamp);
            paint.SetShader(shader);
            paint.AntiAlias = true;

			float r = size / 2f;
			canvas.DrawCircle(r, r, r, paint);

			squaredBitmap.Recycle();
			return bitmap;
        }

        public IResource Transform(Context p0, IResource p1, int p2, int p3)
        {
            throw new NotImplementedException();
        }

        public void UpdateDiskCacheKey(MessageDigest p0)
        {
            throw new NotImplementedException();
        }
    }
}