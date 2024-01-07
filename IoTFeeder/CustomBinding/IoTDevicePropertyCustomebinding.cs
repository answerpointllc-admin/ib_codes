using System;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc;
using System.Collections;
using Kendo.Mvc.Infrastructure;
using IoTFeeder.Common.Models;

namespace IoTFeeder.Admin.CustomBinding
{
    public static class IoTDevicePropertyCustomebinding
    {
        public enum IoTDevicePropertyFieldEnum
        {
            PropertyName,
            DeviceName,
            MininmumValue,
            MaximumValue,
            MaxLength,
            Active
        }

        public static IQueryable<IoTDevicePropertyViewModel> ApplyFiltering(this IQueryable<IoTDevicePropertyViewModel> data, IList<IFilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                data = data.Where(ExpressionBuilder.Expression<IoTDevicePropertyViewModel>(filterDescriptors));
            }
            return data;
        }

        public static IEnumerable ApplyGrouping(this IQueryable<IoTDevicePropertyViewModel> data, IList<GroupDescriptor> groupDescriptors)
        {
            Func<IEnumerable<IoTDevicePropertyViewModel>, IEnumerable<AggregateFunctionsGroup>> selector = null;

            foreach (var group in groupDescriptors.Reverse())
            {
                IoTDevicePropertyFieldEnum UserEnum = GetIoTDevicePropertyFieldEnum(group.Member);
                if (selector == null)
                {
                    if (UserEnum == IoTDevicePropertyFieldEnum.PropertyName)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.PropertyName, group.Member);
                    }
                    else if (UserEnum == IoTDevicePropertyFieldEnum.DeviceName)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.DeviceName, group.Member);
                    }
                    else if (UserEnum == IoTDevicePropertyFieldEnum.MininmumValue)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.MininmumValue, group.Member);
                    }
                    else if (UserEnum == IoTDevicePropertyFieldEnum.MaximumValue)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.MaximumValue, group.Member);
                    }
                    else if (UserEnum == IoTDevicePropertyFieldEnum.MaxLength)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.MaxLength, group.Member);
                    }
                    else if (UserEnum == IoTDevicePropertyFieldEnum.Active)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.Active, group.Member);
                    }
                }
                else
                {
                    if (UserEnum == IoTDevicePropertyFieldEnum.PropertyName)
                    {
                        selector = BuildGroup(o => o.PropertyName, selector, group.Member);
                    }
                    else if (UserEnum == IoTDevicePropertyFieldEnum.DeviceName)
                    {
                        selector = BuildGroup(o => o.DeviceName, selector, group.Member);
                    }
                    else if (UserEnum == IoTDevicePropertyFieldEnum.MininmumValue)
                    {
                        selector = BuildGroup(o => o.MininmumValue, selector, group.Member);
                    }
                    else if (UserEnum == IoTDevicePropertyFieldEnum.MaximumValue)
                    {
                        selector = BuildGroup(o => o.MaximumValue, selector, group.Member);
                    }
                    else if (UserEnum == IoTDevicePropertyFieldEnum.MaxLength)
                    {
                        selector = BuildGroup(o => o.MaxLength, selector, group.Member);
                    }
                    else if (UserEnum == IoTDevicePropertyFieldEnum.Active)
                    {
                        selector = BuildGroup(o => o.Active, selector, group.Member);
                    }
                }
            }
            return selector.Invoke(data).ToList();
        }

        private static Func<IEnumerable<IoTDevicePropertyViewModel>, IEnumerable<AggregateFunctionsGroup>> BuildGroup<T>(Func<IoTDevicePropertyViewModel, T> groupSelector, Func<IEnumerable<IoTDevicePropertyViewModel>, IEnumerable<AggregateFunctionsGroup>> selectorBuilder, string Value)
        {
            var tempSelector = selectorBuilder;

            return g => g.GroupBy(groupSelector)
                         .Select(c => new AggregateFunctionsGroup
                         {
                             Key = c.Key,
                             Member = Value,
                             HasSubgroups = true,
                             Items = tempSelector.Invoke(c).ToList()
                         });
        }

        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<IoTDevicePropertyViewModel> group, Func<IoTDevicePropertyViewModel, T> groupSelector, string Value)
        {
            return group.GroupBy(groupSelector)
                    .Select(i => new AggregateFunctionsGroup
                    {
                        Key = i.Key,
                        Member = Value,
                        Items = i.ToList()
                    });
        }

        public static IQueryable<IoTDevicePropertyViewModel> ApplyPaging(this IQueryable<IoTDevicePropertyViewModel> data, int currentPage, int pageSize)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                data = data.Skip((currentPage - 1) * pageSize);
            }

            data = data.Take(pageSize);
            return data;
        }

        public static IQueryable<IoTDevicePropertyViewModel> ApplySorting(this IQueryable<IoTDevicePropertyViewModel> data, IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
        {
            if (groupDescriptors.Any())
            {
                foreach (var groupDescriptor in groupDescriptors.Reverse())
                {
                    data = AddSortExpression(data, groupDescriptor.SortDirection, groupDescriptor.Member);
                }
            }

            if (sortDescriptors.Any())
            {
                foreach (SortDescriptor sortDescriptor in sortDescriptors)
                {
                    data = AddSortExpression(data, sortDescriptor.SortDirection, sortDescriptor.Member);
                }
            }
            return data;
        }

        private static IQueryable<IoTDevicePropertyViewModel> AddSortExpression(IQueryable<IoTDevicePropertyViewModel> data, Kendo.Mvc.ListSortDirection sortDirection, string memberName)
        {
            IoTDevicePropertyFieldEnum UserEnum = GetIoTDevicePropertyFieldEnum(memberName);
            if (sortDirection == Kendo.Mvc.ListSortDirection.Ascending)
            {
                switch (UserEnum)
                {
                    case IoTDevicePropertyFieldEnum.PropertyName:
                        data = data.OrderBy(order => order.PropertyName);
                        break;
                    case IoTDevicePropertyFieldEnum.DeviceName:
                        data = data.OrderBy(order => order.DeviceName);
                        break;
                    case IoTDevicePropertyFieldEnum.MininmumValue:
                        data = data.OrderBy(order => order.MininmumValue);
                        break;
                    case IoTDevicePropertyFieldEnum.MaximumValue:
                        data = data.OrderBy(order => order.MaximumValue);
                        break;
                    case IoTDevicePropertyFieldEnum.MaxLength:
                        data = data.OrderBy(order => order.MaxLength);
                        break;
                    case IoTDevicePropertyFieldEnum.Active:
                        data = data.OrderBy(order => order.Active);
                        break;
                }
            }
            else
            {
                switch (UserEnum)
                {
                    case IoTDevicePropertyFieldEnum.PropertyName:
                        data = data.OrderByDescending(order => order.PropertyName);
                        break;
                    case IoTDevicePropertyFieldEnum.DeviceName:
                        data = data.OrderByDescending(order => order.DeviceName);
                        break;
                    case IoTDevicePropertyFieldEnum.MininmumValue:
                        data = data.OrderByDescending(order => order.MininmumValue);
                        break;
                    case IoTDevicePropertyFieldEnum.MaximumValue:
                        data = data.OrderByDescending(order => order.MaximumValue);
                        break;
                    case IoTDevicePropertyFieldEnum.MaxLength:
                        data = data.OrderByDescending(order => order.MaxLength);
                        break;
                    case IoTDevicePropertyFieldEnum.Active:
                        data = data.OrderByDescending(order => order.Active);
                        break;
                }
            }
            return data;
        }

        private static IoTDevicePropertyFieldEnum GetIoTDevicePropertyFieldEnum(string FieldValue)
        {
            return (IoTDevicePropertyFieldEnum)Enum.Parse(typeof(IoTDevicePropertyFieldEnum), FieldValue);
        }
    }
}