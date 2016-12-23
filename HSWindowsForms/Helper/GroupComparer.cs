using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSCore;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI;

namespace HSWindowsForms.Helper
{
    public class GroupComparer : IComparer<Group<GridViewRowInfo>>
    {
        public int Compare(Group<GridViewRowInfo> x, Group<GridViewRowInfo> y)
        {
            SetEnum parsedX = Enums.GetValueFromDescription<SetEnum>(((object[]) x.Key).First().ToString());
            SetEnum parsedY = Enums.GetValueFromDescription<SetEnum>(((object[]) y.Key).First().ToString());

            if (parsedX != default(SetEnum) && parsedY != default(SetEnum))
            {
                int result = parsedX.CompareTo(parsedY);
                DataGroup xGroup = x as DataGroup;
                if (xGroup != null && ((DataGroup)x).GroupDescriptor.GroupNames.First().Direction == ListSortDirection.Descending)
                {
                    result *= -1;
                }
                return result;
            }
            return string.Compare(((object[])x.Key)[0].ToString(), ((object[])y.Key)[0].ToString(), StringComparison.Ordinal);
        }
    }
}
