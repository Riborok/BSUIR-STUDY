const toJSON = (doc) => {
    if (!doc) return null;
    const obj = doc.toObject ? doc.toObject() : doc;
    const { _id, __v, ...rest } = obj;
    return { id: _id.toString(), ...rest };
};

export const mapUser = (user) => {
    if (!user) return null;
    const json = toJSON(user);

    if (json.projects) {
        json.projects = json.projects.map(p => p.toString());
    }
    if (json.tasks) {
        json.tasks = json.tasks.map(t => t.toString());
    }

    return json;
};

export const mapProject = (project) => {
    if (!project) return null;
    const json = toJSON(project);

    if (json.users && Array.isArray(json.users)) {
        json.users = json.users.map(user => {
            if (typeof user === 'object' && user._id) {
                return mapUser(user);
            }
            return user.toString();
        });
    }

    return json;
};

export const mapTask = (task) => {
    if (!task) return null;
    const json = toJSON(task);

    if (json.projectId) {
        if (typeof json.projectId === 'object' && json.projectId._id) {
            json.projectId = json.projectId._id.toString();
        } else {
            json.projectId = json.projectId.toString();
        }
    }

    if (json.user) {
        if (typeof json.user === 'object' && json.user._id) {
            json.user = mapUser(json.user);
        } else {
            json.user = json.user.toString();
        }
    }

    return json;
};

export const mapMany = (docs, mapper) => {
    if (!Array.isArray(docs)) return [];
    return docs.map(mapper);
};
