// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/**
     *
     * todo: Resize element height
     * event: 'resize'
     * returns: height
     * requires: class 'nu-auto-height'
     * config: You can override offet by using this class 'nu-height-offset-{number}'
     *
     */
window.globalElementHeight = function () {
    const elements = document.querySelectorAll('.nu-auto-height');

    elements.forEach(element => {

        let offset = 150; // Default offset
        const offsetClass = Array.from(element.classList).find(cls => cls.startsWith('nu-height-offset-'));

        // override default offset if offsetHeight class is specified 'nu-offset-120'
        if (offsetClass) {
            const match = offsetClass.match(/nu-height-offset-(\d+)/);
            if (match) {
                offset = parseInt(match[1], 10);
            }
        }

        const newHeight = window.innerHeight - offset;

        if (element.ej2_instances && element.ej2_instances[0]) {
            const instance = element.ej2_instances[0];

            if ('height' in instance && typeof instance.refresh === 'function') {
                instance.height = `${newHeight}px`;
                //instance.resize();
            }
        }
    });
};

// Automatically adjust on window resize
window.addEventListener('resize', window.globalElementHeight);

// Initial call on DOM load
document.addEventListener('DOMContentLoaded', () => {
    window.globalElementHeight();
});