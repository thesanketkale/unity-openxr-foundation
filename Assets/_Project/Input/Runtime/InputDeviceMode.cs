namespace Foundation.Input
{
    /// <summary>
    /// Operating modes for XR input sources.
    /// </summary>
    public enum InputDeviceMode
    {
        /// <summary>
        /// Automatically switches between controllers and optical hand tracking based on tracking confidence.
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// Forces motion controllers only; ignores hand tracking inputs.
        /// </summary>
        ControllerOnly = 1,

        /// <summary>
        /// Forces optical hand tracking only; ignores physical controllers.
        /// </summary>
        HandsOnly = 2
    }
}
