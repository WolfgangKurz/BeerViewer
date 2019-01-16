interface IDisposable {
    Dispose(): void;
}

function using<T extends IDisposable>(resource: T, func: (resource: T) => void) {
    try {
        func(resource);
    } finally {
        resource.Dispose();
    }
}
