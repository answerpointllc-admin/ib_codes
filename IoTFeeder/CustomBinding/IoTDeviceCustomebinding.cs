using System;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc;
using System.Collections;
using Kendo.Mvc.Infrastructure;
using IoTFeeder.Common.Models;

namespace IoTFeeder.Admin.CustomBinding
{
    public static class IoTDeviceCustomebinding
    {
        public enum IoTDeviceFields
        {
            DeviceName,
            Description,
            FrequencyTypeText,
            MinValue,
            MaxValue,
            Frequency,
            Active
        }

        public static IQueryable<IoTDeviceViewModel> ApplyFiltering(this IQueryable<IoTDeviceViewModel> data, IList<IFilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                data = data.Where(ExpressionBuilder.Expression<IoTDeviceViewModel>(filterDescriptors));
            }
            return data;
        }

        public static IEnumerable ApplyGrouping(this IQueryable<IoTDeviceViewModel> data, IList<GroupDescriptor> groupDescriptors)
        {
            Func<IEnumerable<IoTDeviceViewModel>, IEnumerable<AggregateFunctionsGroup>> selector = null;

            foreach (var group in groupDescriptors.Reverse())
            {
                IoTDeviceFields UserEnum = GetIoTDeviceFieldEnum(group.Member);
                if (selector == null)
                {
                    if (UserEnum == IoTDeviceFields.DeviceName)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.DeviceName, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.Description)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.Description, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.FrequencyTypeText)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.FrequencyTypeText, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.MinValue)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.MinValue, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.MaxValue)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.MaxValue, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.Frequency)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.Frequency, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.Active)
                    {
                        selector = orders => BuildInnerGroup(orders, o => o.Active, group.Member);
                    }
                }
                else
                {
                    if (UserEnum == IoTDeviceFields.DeviceName)
                    {
                        selector = BuildGroup(o => o.DeviceName, selector, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.Description)
                    {
                        selector = BuildGroup(o => o.Description, selector, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.FrequencyTypeText)
                    {
                        selector = BuildGroup(o => o.FrequencyTypeText, selector, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.MinValue)
                    {
                        selector = BuildGroup(o => o.MinValue, selector, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.MaxValue)
                    {
                        selector = BuildGroup(o => o.MaxValue, selector, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.Frequency)
                    {
                        selector = BuildGroup(o => o.Frequency, selector, group.Member);
                    }
                    else if (UserEnum == IoTDeviceFields.Active)
                    {
                        selector = BuildGroup(o => o.Active, selector, group.Member);
                    }
                }
            }
            return selector.Invoke(data).ToList();
        }

        private static Func<IEnumerable<IoTDeviceViewModel>, IEnumerable<AggregateFunctionsGroup>> BuildGroup<T>(Func<IoTDeviceViewModel, T> groupSelector, Func<IEnumerable<IoTDeviceViewModel>, IEnumerable<AggregateFunctionsGroup>> selectorBuilder, string Value)
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

        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<IoTDeviceViewModel> group, Func<IoTDeviceViewModel, T> groupSelector, string Value)
        {
            return group.GroupBy(groupSelector)
                    .Select(i => new AggregateFunctionsGroup
                    {
                        Key = i.Key,
                        Member = Value,
                        Items = i.ToList()
                    });
        }

        public static IQueryable<IoTDeviceViewModel> ApplyPaging(this IQueryable<IoTDeviceViewModel> data, int currentPage, int pageSize)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                data = data.Skip((currentPage - 1) * pageSize);
            }

            data = data.Take(pageSize);
            return data;
        }

        public static IQueryable<IoTDeviceViewModel> ApplySorting(this IQueryable<IoTDeviceViewModel> data, IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
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

        private static IQueryable<IoTDeviceViewModel> AddSortExpression(IQueryable<IoTDeviceViewModel> data, Kendo.Mvc.ListSortDirection sortDirection, string memberName)
        {
            IoTDeviceFields UserEnum = GetIoTDeviceFieldEnum(memberName);
            if (sortDirection == Kendo.Mvc.ListSortDirection.Ascending)
            {
                switch (UserEnum)
                {
                    case IoTDeviceFields.DeviceName:
                        data = data.OrderBy(order => order.DeviceName);
                        break;
                    case IoTDeviceFields.Description:
                        data = data.OrderBy(order => order.Description);
                        break;
                    case IoTDeviceFields.FrequencyTypeText:
                        data = data.OrderBy(order => order.FrequencyTypeText);
                        break;
                    case IoTDeviceFields.MinValue:
                        data = data.OrderBy(order => order.MinValue);
                        break;
                    case IoTDeviceFields.MaxValue:
                        data = data.OrderBy(order => order.MaxValue);
                        break;
                    case IoTDeviceFields.Frequency:
                        data = data.OrderBy(order => order.Frequency);
                        break;

                    case IoTDeviceFields.Active:
                        data = data.OrderBy(order => order.Active);
                        break;
                }
            }
            else
            {
                switch (UserEnum)
                {
                    case IoTDeviceFields.DeviceName:
                        data = data.OrderByDescending(order => order.DeviceName);
                        break;
                    case IoTDeviceFields.Description:
                        data = data.OrderByDescending(order => order.Description);
                        break;
                    case IoTDeviceFields.FrequencyTypeText:
                        data = data.OrderByDescending(order => order.FrequencyTypeText);
                        break;
                    case IoTDeviceFields.MinValue:
                        data = data.OrderByDescending(order => order.MinValue);
                        break;
                    case IoTDeviceFields.MaxValue:
                        data = data.OrderByDescending(order => order.MaxValue);
                        break;
                    case IoTDeviceFields.Frequency:
                        data = data.OrderByDescending(order => order.Frequency);
                        break;
                    case IoTDeviceFields.Active:
                        data = data.OrderByDescending(order => order.Active);
                        break;
                }
            }
            return data;
        }

        private static IoTDeviceFields GetIoTDeviceFieldEnum(string FieldValue)
        {
            return (IoTDeviceFields)Enum.Parse(typeof(IoTDeviceFields), FieldValue);
        }
    }
}