using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;

namespace FINN.SLIDE;

public static class OpenXmlWrapper
{
    /// <summary>
    ///     Copy and paste a slide from one into another.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="fromIndex"></param>
    /// <param name="toIndex"></param>
    public static void Copy(PresentationDocument from, PresentationDocument to, int fromIndex, int? toIndex = null)
    {
        // copy the SlidePart from source to target document
        var slidePart = to.PresentationPart.AddPart(from.PresentationPart.GetPartById(
            (from.PresentationPart.Presentation.SlideIdList.ChildElements[fromIndex] as SlideId)
            .RelationshipId) as SlidePart);

        // copy the SlideMasterPart from source to target document
        var slideMasterPart = to.PresentationPart.AddPart(slidePart.SlideLayoutPart.SlideMasterPart);

        // manipulate presentation.xml
        // append new slideId node to presentation.xml
        var slideId = new SlideId
        {
            Id = GetMaxId(to.PresentationPart.Presentation.SlideIdList) + 1,
            RelationshipId = to.PresentationPart.GetIdOfPart(slidePart)
        };
        to.PresentationPart.Presentation.SlideIdList.InsertAt(slideId,
            toIndex ?? to.PresentationPart.Presentation.SlideIdList.ChildElements.Count);

        // append new slideMasterId node to presentation.xml
        var slideMasterId = new SlideMasterId
        {
            Id = GetMaxId(to.PresentationPart.Presentation.SlideMasterIdList) + 1,
            RelationshipId = to.PresentationPart.GetIdOfPart(slideMasterPart)
        };
        to.PresentationPart.Presentation.SlideMasterIdList.Append(slideMasterId);
        FixLayoutId(to, slideMasterId.Id);

        // save the presentation.xml
        to.PresentationPart.Presentation.Save();
    }

    private static UInt32Value GetMaxId(TypedOpenXmlCompositeElement compositeElement)
    {
        if (compositeElement.ChildElements.Count == 0)
            return new UInt32Value((uint)255);


        return compositeElement switch
        {
            SlideMasterIdList => compositeElement.ChildElements.Cast<SlideMasterId>().Max(x => x.Id),
            SlideIdList => compositeElement.ChildElements.Cast<SlideId>().Max(x => x.Id),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static void FixLayoutId(PresentationDocument document, uint slideMasterIdValue)
    {
        foreach (var slideMasterPart in document.PresentationPart!.SlideMasterParts)
        {
            foreach (SlideLayoutId slideLayoutId in slideMasterPart.SlideMaster.SlideLayoutIdList!)
            {
                slideMasterIdValue++;
                slideLayoutId.Id = slideMasterIdValue;
            }

            slideMasterPart.SlideMaster.Save();
        }
    }
}