using System.Drawing.Design;

namespace DynamicPropertyObject
{
    public class PropertyValuePaintEditor : UITypeEditor
    {
        public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            // let the property browser know we'd like
            // to do custom painting.
            if (context != null && context.PropertyDescriptor is DynPropertyDescriptor)
            {
                var pd = (DynPropertyDescriptor) context.PropertyDescriptor;
                return (pd.ValueImage != null);
            }
            return base.GetPaintValueSupported(context);
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.None;
        }

        public override void PaintValue(PaintValueEventArgs pe)
        {
            if (pe.Context != null && pe.Context.PropertyDescriptor != null && pe.Context.PropertyDescriptor is DynPropertyDescriptor pd && pd.ValueImage != null)
            {
                pe.Graphics.DrawImage(pd.ValueImage, pe.Bounds);
                return;
            }
            base.PaintValue(pe);
        }
    }
}