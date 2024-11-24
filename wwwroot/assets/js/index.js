function main() {

    document.addEventListener("DOMContentLoaded", (arg, ...args) => {
        console.log("DOM CONTENT LOADED");
        console.log(arg);
        console.log(args);
        const hiddenFrame = document.getElementById("hiddenFrame");
        hiddenFrame.addEventListener("load", (arg, ...args) => {
            console.log(arg);
            const guid = getSubmittedGuid(arg.explicitOriginalTarget);
            updateRetrieveField(guid);
            console.log(args);
        })
    });

};

/**
 * 
 * @returns {string} string encoded GUID (UUID v4) of the most recently submitted object by this client
 */
function getSubmittedGuid(explicitOriginalTarget) {
    const object = JSON.parse(explicitOriginalTarget.contentDocument.children[0].children[1].children[0].textContent);
    return object.guid;
}

function updateRetrieveField(guid) {
    console.log(`guid = ${guid}`);
    const inputField = document.getElementById("guid-form-entry-text");
    console.log("inputField = ", inputField);
    inputField.value = guid;
}

main();