using System;

namespace Districts.New.Interfaces
{
    [Flags]
    public enum ReturnConditions
    {
        SameHouse = 1,
        SameHousing = SameHouse | 2,
        WithSlash = SameHousing | 4,

        Less = 8,
        More = 16,


        //LessThenInclude = LessThen | Self,
        //MoreThenInclude = MoreThen | Self,

        //All = LessThen | LessThenInclude | MoreThen | MoreThenInclude | Self,
        //AllWithSlashCheck = All | CompareSlash
    }

    interface iFind
    {
        string Street { get; }
        int HomeNumber { get; }
        int Housing { get; }
        int AfterSlash { get; }

        bool SameObject(iFind obj, ReturnConditions conditions = ReturnConditions.WithSlash);
    }
}
