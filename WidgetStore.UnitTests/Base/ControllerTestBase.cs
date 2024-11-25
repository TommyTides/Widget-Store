using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WidgetStore.UnitTests.Base
{
    /// <summary>
    /// Base class for controller tests providing common functionality
    /// </summary>
    public abstract class ControllerTestBase
    {
        /// <summary>
        /// Sets up basic controller context
        /// </summary>
        /// <param name="controller">Controller to configure</param>
        protected static void SetupControllerContext(ControllerBase controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }
    }
}