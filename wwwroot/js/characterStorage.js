// ES module wrapping the browser IndexedDB API for local-only character persistence.
// No network calls of any kind belong in this file.

const DATABASE_NAME = "dnd-sheets";
const DATABASE_VERSION = 1;
const CHARACTERS_STORE = "characters";
const PORTRAITS_STORE = "portraits";

/** @type {Promise<IDBDatabase> | null} */
let databasePromise = null;

/**
 * Opens (or creates) the dnd-sheets database, creating both object stores
 * the first time the database version is initialized.
 * @returns {Promise<IDBDatabase>}
 */
function openDatabase() {
    if (databasePromise) {
        return databasePromise;
    }

    databasePromise = new Promise((resolve, reject) => {
        const request = indexedDB.open(DATABASE_NAME, DATABASE_VERSION);

        request.onupgradeneeded = () => {
            const database = request.result;

            if (!database.objectStoreNames.contains(CHARACTERS_STORE)) {
                database.createObjectStore(CHARACTERS_STORE, { keyPath: "id" });
            }

            if (!database.objectStoreNames.contains(PORTRAITS_STORE)) {
                database.createObjectStore(PORTRAITS_STORE, { keyPath: "id" });
            }
        };

        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });

    return databasePromise;
}

/**
 * Wraps a single IDBRequest in a Promise, resolving with its result or
 * rejecting with its error so failures surface as a JSException in .NET.
 * @param {IDBRequest} request
 * @returns {Promise<any>}
 */
function requestToPromise(request) {
    return new Promise((resolve, reject) => {
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}

/**
 * Creates or updates a character record.
 * @param {string} id
 * @param {string} displayName
 * @param {string | null} portraitReference
 * @param {string} json
 * @returns {Promise<void>}
 */
export async function saveCharacter(id, displayName, portraitReference, json) {
    const database = await openDatabase();
    const transaction = database.transaction(CHARACTERS_STORE, "readwrite");
    const store = transaction.objectStore(CHARACTERS_STORE);
    await requestToPromise(store.put({ id, displayName, portraitReference, json }));
}

/**
 * Reads a character's authoritative JSON by id.
 * @param {string} id
 * @returns {Promise<string | null>}
 */
export async function getCharacter(id) {
    const database = await openDatabase();
    const transaction = database.transaction(CHARACTERS_STORE, "readonly");
    const store = transaction.objectStore(CHARACTERS_STORE);
    const record = await requestToPromise(store.get(id));
    return record ? record.json : null;
}

/**
 * Lists every character's list-view projection without parsing any JSON.
 * @returns {Promise<Array<{id: string, displayName: string, portraitReference: string | null}>>}
 */
export async function listCharacters() {
    const database = await openDatabase();
    const transaction = database.transaction(CHARACTERS_STORE, "readonly");
    const store = transaction.objectStore(CHARACTERS_STORE);
    const records = await requestToPromise(store.getAll());
    return records.map((record) => ({
        id: record.id,
        displayName: record.displayName,
        portraitReference: record.portraitReference,
    }));
}

/**
 * Deletes a character record by id.
 * @param {string} id
 * @returns {Promise<void>}
 */
export async function deleteCharacter(id) {
    const database = await openDatabase();
    const transaction = database.transaction(CHARACTERS_STORE, "readwrite");
    const store = transaction.objectStore(CHARACTERS_STORE);
    await requestToPromise(store.delete(id));
}

/**
 * Creates or updates a portrait record.
 * @param {string} id
 * @param {string} contentType
 * @param {string} base64
 * @returns {Promise<void>}
 */
export async function savePortrait(id, contentType, base64) {
    const database = await openDatabase();
    const transaction = database.transaction(PORTRAITS_STORE, "readwrite");
    const store = transaction.objectStore(PORTRAITS_STORE);
    await requestToPromise(store.put({ id, contentType, base64 }));
}

/**
 * Reads a portrait record by id.
 * @param {string} id
 * @returns {Promise<{contentType: string, base64: string} | null>}
 */
export async function getPortrait(id) {
    const database = await openDatabase();
    const transaction = database.transaction(PORTRAITS_STORE, "readonly");
    const store = transaction.objectStore(PORTRAITS_STORE);
    const record = await requestToPromise(store.get(id));
    return record ? { contentType: record.contentType, base64: record.base64 } : null;
}

/**
 * Deletes a portrait record by id.
 * @param {string} id
 * @returns {Promise<void>}
 */
export async function deletePortrait(id) {
    const database = await openDatabase();
    const transaction = database.transaction(PORTRAITS_STORE, "readwrite");
    const store = transaction.objectStore(PORTRAITS_STORE);
    await requestToPromise(store.delete(id));
}
