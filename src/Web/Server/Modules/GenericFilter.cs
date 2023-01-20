using AutoFilterer.Enums;
using AutoFilterer.Types;

namespace XClaim.Web.Server.Modules;

public class GenericFilter : PaginationFilterBase {
    protected GenericFilter() {
        Page = 1;
        PerPage = 25;
        SortBy = Sorting.Ascending;
        CombineWith = CombineType.Or;
    }
    public sealed override int Page { get; set; }
    public override string? Sort { get; set; }
    public sealed override int PerPage { get; set; }
    public sealed override Sorting SortBy { get; set; }
    public sealed override CombineType CombineWith { get; set; }
}