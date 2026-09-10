// ES module for triggering a browser download of in-memory JSON. Entirely local - no network
// calls of any kind belong in this file.

/**
 * Builds a JSON blob and triggers a browser download of it under the given file name.
 * @param {string} fileName
 * @param {string} json
 * @returns {Promise<void>}
 */
export async function downloadJson(fileName, json) {
    const blob = new Blob([json], { type: "application/json" });
    const url = URL.createObjectURL(blob);

    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = fileName;
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);

    URL.revokeObjectURL(url);
}
