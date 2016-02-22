import os
import pickle
import re


# old-style classes
# new-style classes

class Store(object):
    def __init__(self):
        self._container = set()
        self._DATA_PATH = 'data.dat'

    def add(self, *items):
        for item in items:
            self._container.add(item)

    def remove(self, item):
        self._container.discard(item)

    def grep(self, regex):
        res = []
        for item in self._container:
            if re.match(regex, str(item)) is not None:
                res.append(item)
        return res

    def load(self):
        if os.path.exists(self._DATA_PATH):
            try:
                with open(self._DATA_PATH, 'rb') as f:
                    self._container = pickle.load(f)
            except:
                raise ValueError('Bad data file schema')
        else:
            raise IOError('File not exist')

    def save(self):
        with open(self._DATA_PATH, 'wb') as f:
            pickle.dump(self._container, f)

    def find(self, *items):
        res = []
        for item in items:
            if item in self._container:
                res.append(item)

        return res

    def items(self):
        for item in self._container:
            yield item
