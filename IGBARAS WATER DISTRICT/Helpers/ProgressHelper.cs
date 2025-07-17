using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class ProgressHelper
    {
        public static async Task ShowProgressAsync(
            Panel loadingPanel,
            ProgressBar progressBar,
            Func<Task> backgroundTask // The task you want to run
        )
        {
            if (loadingPanel == null || progressBar == null || backgroundTask == null)
                return;

            if (!loadingPanel.IsHandleCreated || !progressBar.IsHandleCreated)
                return;

            // Show panel and reset progress
            loadingPanel.Invoke(() =>
            {
                progressBar.Value = 0;
                loadingPanel.BringToFront();
                loadingPanel.Visible = true;
            });

            var progress = new Progress<int>(value =>
            {
                if (progressBar.IsHandleCreated)
                    progressBar.Value = value;
            });

            var simulateProgressTask = SimulateProgress(progress);

            // Run your background task in parallel
            await Task.WhenAll(backgroundTask(), simulateProgressTask);

            // Hide the panel when done
            loadingPanel.Invoke(() => loadingPanel.Visible = false);
        }

        // Simulate smooth progress from 0 to 100 over ~1.5s
        private static async Task SimulateProgress(IProgress<int> progress)
        {
            for (int i = 0; i <= 100; i++)
            {
                progress.Report(i);
                await Task.Delay(15); // Adjust speed here
            }
        }
    }
}
