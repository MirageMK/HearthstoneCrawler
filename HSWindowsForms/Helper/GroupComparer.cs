using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HSCore;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI;

namespace HSWindowsForms.Helper
{
    public class GroupComparer : IComparer<Group<GridViewRowInfo>>
    {
        public int Compare(Group<GridViewRowInfo> x, Group<GridViewRowInfo> y)
        {
            string xString = ((object[]) x.Key).First().ToString();
            string yString = ((object[]) y.Key).First().ToString();

            if(xString == "Neutral")
                return 9999;
            if(yString == "Neutral")
                return -9999;

            SetEnum parsedSetX = Enums.GetValueFromDescription<SetEnum>(xString);
            SetEnum parsedSetY = Enums.GetValueFromDescription<SetEnum>(yString);

            if(parsedSetX != default(SetEnum) && parsedSetY != default(SetEnum))
            {
                int result = parsedSetX.CompareTo(parsedSetY);
                DataGroup xGroup = x as DataGroup;
                if(xGroup != null && ((DataGroup) x).GroupDescriptor.GroupNames.First().Direction == ListSortDirection.Descending)
                    result *= -1;
                return result;
            }
            return string.Compare(xString, yString, StringComparison.Ordinal);
        }
    }
}