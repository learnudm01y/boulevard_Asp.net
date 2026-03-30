using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class ProjectPlanResponseDto
    {
        public int ProjectPlanId { get; set; }
        public int? ServiceTypeId { get; set; }
       
        public string Title { get; set; }
        public string UniteType { get; set; }
        public string Image { get; set; }
    }

    public class ProjectPlanByTitleResponse
    {
        public string Title { get; set; }
        public List<ProjectPlanResponseDto> Plans { get; set; }
    }
}