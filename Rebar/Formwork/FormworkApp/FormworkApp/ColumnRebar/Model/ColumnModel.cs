using Autodesk.Revit.DB;

namespace FormworkApp.ColumnRebar.Model
{
    public class ColumnModel
    {
        public XYZ A { get; set; }
        public XYZ B { get; set; }
        public XYZ C { get; set; }
        public XYZ D { get; set; }
        public XYZ Origin { get; set; }
        public double BaseElevation { get; set; }
        public double TopElevation { get; set; }
        public Transform Transform { get; set; }
        public XYZ RightVector { get; set; }
        public XYZ UpVector { get; set; }
        public double SectionWidth { get; set; }
        public double SectionHeight { get; set; }

        public FamilyInstance Column { get; set; }
        public ColumnModel(FamilyInstance column)
        {
            Column = column;
            Transform = column.GetTransform();
            RightVector = Transform.BasisX;
            UpVector = Transform.BasisY;
            Origin = Transform.Origin;
            var symbol = column.Symbol;
            SectionWidth = symbol.LookupParameter("b").AsDouble();
            SectionHeight = symbol.LookupParameter("h").AsDouble();
            var bb = column.get_BoundingBox(null);
            TopElevation = bb.Max.Z;
            BaseElevation = bb.Min.Z;

            A = Transform.OfPoint(new XYZ(-SectionWidth / 2, SectionHeight / 2, BaseElevation));
            B = Transform.OfPoint(new XYZ(SectionWidth / 2, SectionHeight / 2, BaseElevation));
            C = Transform.OfPoint(new XYZ(SectionWidth / 2, -SectionHeight / 2, BaseElevation));
            D = Transform.OfPoint(new XYZ(-SectionWidth / 2, -SectionHeight / 2, BaseElevation));

            Origin = Transform.Origin;
        }

    }
}
