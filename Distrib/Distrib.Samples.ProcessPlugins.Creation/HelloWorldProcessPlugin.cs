/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.PluginPowered;
using Distrib.Processes.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Samples.ProcessPlugins.Creation
{
    /*
     * Here we decorate the class to signal it's a plugin.
     * The attribute takes some pretty standard details.
     * 
     * The attribute is required for use with the plugin system,
     * the metadata provided to it is mirrored and used by the process
     * system to describe the process
     */
    [DistribProcessPlugin(
        name: "Hello World",
        description: "Simple process plugin that say's hello world!",
        version: 1.0,
        author: "Clint Pearson",
        identifier: "{1AA1405C-CBF4-41EC-9D93-EFE7F5213E6B}")]
    public sealed class HelloWorldProcessPlugin :
        CrossAppDomainObject,   /* Derive from CrossAppDomainObject in order to ensure that this object doesn't expire owing to inactivity across domain boundaries */
        IPlugin,                /* Implement IPlugin to hook into the plugin system, this interface is required for use with the plugin system */
        IProcess                /* Implement IProcess to hook into the process system, this interface is required for all processes */
    {
        void IPlugin.InitialisePlugin(IPluginInteractionLink interactionLink)
        {
            /*
             * This method is called when the plugin system creates the instance of your plugin
             * and is giving you a chance to do plugin-specific things before
             * it is handed off to the system using it (in this case the process system)
             */
        }

        void IPlugin.UninitialisePlugin(IPluginInteractionLink interactionLink)
        {
            /*
             * This method is called when the plugin system is getting ready
             * to tear down your plugin instance, this is the last chance
             * you have for performing clean-up while still having access
             * to the plugin system
             */
        }

        void IProcess.InitProcess()
        {
            /*
             * Once the process system has hold of your instance it calls this method
             * to give you chance to perform any initialisation you may need to do
             * before your process is ready for use
             */
        }

        void IProcess.UninitProcess()
        {
            /*
             * This method is called when the process system is getting ready to
             * tear down your instance.
             */
        }

        /* This is just here to store the hello world job definition, and allow us to lazy initialise
         * and cache it
         */
        private ProcessJobDefinition<IStockNoInput, IStockOutput<string>> _helloWorldJob = null;


        /*
         * The process system requests this property to discover what jobs
         * your process exposes.
         * 
         * NOTE:
         * This property may be requested multiple times, and while it's
         * possible for you to manipulate the definitions across
         * multiple requests there is no guarantee for how the system will react.
         * - You may end up with jobs not executing at all, jobs failing and all manner of problems.
         * 
         */
        IReadOnlyList<IJobDefinition> IProcess.JobDefinitions
        {
            get
            {
                if (_helloWorldJob == null)
                {
                    /*
                     * Here we create the job definition for the hello world job
                     * of note here:
                     * > IStockNoInput
                     *      This is the input interface type, because we say hello world
                     *      we're not taking input, Distrib has some stock input and output
                     *      interfaces for use.
                     * > IStockOutput<string>
                     *      This is the output interface type, we're returning
                     *      a string "Hello World" so we're using the stock output
                     *      interface and just having it return a single thing
                     *      - in this case a string.
                     *  
                     * We also provide a job name and description
                     */
                    _helloWorldJob =
                        new ProcessJobDefinition<IStockNoInput, IStockOutput<string>>(
                            "Hello World", "Says hello world");
                }

                return new[] { _helloWorldJob };
            }
        }

        /*
         * This method gets called whenever the process system has a job
         * for your process to execute
         */
        void IProcess.ProcessJob(IJob job)
        {
            /*
             * Here we're given an IJob to work with, this holds all the information
             * we need to perform the job and to grab inputs and provide outputs.
             * 
             * IJob is a little low-level and is quite manual to work with, so there
             * are some built-in helpers for working with IJob.
             * 
             * We use the JobExecutionHelper.New() method to spawn a new execution helper
             * > JobExecutionHelper works with jobs in the form of helpers
             *   each helper takes a function for checking against a job definition
             *   and an action to invoke if the job matches that definition.
             *   
             * > JobExecutionHelper then has an execute method which takes a job definition
             *   you just provide it with the definition against the job.
             */

            JobExecutionHelper.New()
                .AddHandler(() => _helloWorldJob,
                    () =>
                    {
                        /*
                         * Here is where we end up for the hello world job.
                         * Jobs carry input and output with them via tracker objects
                         * that deal with default value and config etc,
                         * these are IJob.OutputTracker and IJob.InputTracker.
                         * 
                         * They can be a little difficult to work with directly as they
                         * require type information and a string name for the property;
                         * as a result it's easier to use the data helper class
                         * which deals with that for you.
                         * 
                         * As below:
                         *  > Create a new data helper using the IStockOutput<string> data interface
                         *      > This takes the job definition to get hold of the actual fields
                         *        and any additional config
                         *  > Tell the helper we're using it to set the output of the job
                         *      > The job is supplied so it can actually do this for us
                         *  > Call the set method and it takes a lambda pointing to the property
                         *      > The lambda already knows the type (IStockOutput<string>), you just need
                         *        to provide a parameter (o) and just point to the property, it'll do the rest.
                         *      > A value must also be supplied, this is also strongly-typed so you get nifty
                         *        compiler checking against it.
                         */
                        JobDataHelper<IStockOutput<string>>.New(job.Definition)
                            .ForOutputSet(job)
                            .Set(o => o.Output, "Hello World!");

                        /*
                         * At this point we're finished processing the hello world job.
                         */
                    })
                .Execute(job.Definition);

            /*
             * Now we've gotten to the end of the method, once this method returns it's counted
             * as processing complete for the given job.
             */
        }
    }
}
