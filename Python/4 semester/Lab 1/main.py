import argparse
import re

import numpy as np

from fibonacci import Fibonacci
from sorter import Sorter
from store import Store
from textparser import TextParser


def go_regex():
    print re.match('[+-]?\d+(\.\d*)?', '01')
    print re.match('^[a-z0-9.]+(?:\.[a-z0-9-_]+)*@'
                   '([a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*'
                   '(?:[a-z][a-z][a-z]|[a-z][a-z])$', 'dmitryfisko@gmail.com')

    pattern = r'^((?P<scheme>\w+)://)?' \
              r'(?P<host>([a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*(?:[a-z][a-z][a-z]|[a-z][a-z]))\/' \
              r'(?P<python_version>[0-9]+)\/' \
              r'(?P<folder>[a-z-]+)\/' \
              r'(?P<page_name>[a-z-]+)\.' \
              r'(?P<page_type>[a-z]+)'
    m = re.match(pattern, r'https://docs.python.org/2/library/r-e.html')
    if m is not None:
        print 'Url parameters:'
        print 'scheme: {0}'.format(m.group('scheme'))
        print 'host: {0}'.format(m.group('host'))
        print 'python_version: {0}'.format(m.group('python_version'))
        print 'folder: {0}'.format(m.group('folder'))
        print 'page_name: {0}'.format(m.group('page_name'))
        print 'page_type: {0}'.format(m.group('page_type'))


def go_text():
    parser = TextParser(args.file)

    print 'Text sentences mean {0:.3f}'.format(parser.sentences_mean())
    print 'Text sentences median {0}'.format(parser.sentences_median())
    print parser.top_n_grams()
    print parser['a']


def go_sort():
    arr = np.random.randint(1000, size=10).tolist()
    compare_arr = sorted(arr)
    sorter = Sorter()
    assert sorter.sort(arr) == compare_arr
    assert sorter.sort(arr, 'merge') == compare_arr
    assert sorter.sort(arr, 'radix') == compare_arr

    print sorter.sort(arr, args.mode)


def go_store():
    store = Store()

    store.remove(None)
    store.add(323, 2, 10, 88, 78, 415, 89, 189)
    store.remove(78)

    store.save()
    store.load()

    print store.grep('\d*[02468]$')
    print store.find(89, -1, 415)

    for item in store.items():
        print item


def go_fibonnaci():
    for ind, fib in enumerate(Fibonacci(5)):
        print 'Fibonacci #{ind} = {fib}'.format(ind=ind + 1, fib=fib)


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('action', help='Action type')
    parser.add_argument('-f', '--file', help='File name')
    parser.add_argument('-m', '--mode', help='Sorting mode')
    args = parser.parse_args('fibonacci'.split())
    args = parser.parse_args('sort --mode=quick'.split())
    args = parser.parse_args('text --file=text.txt'.split())
    args = parser.parse_args('store'.split())
    args = parser.parse_args('regex'.split())
    # args = parser.parse_args()

    action = {
        'text': go_text,
        'sort': go_sort,
        'store': go_store,
        'fibonacci': go_fibonnaci,
        'regex': go_regex
    }.get(args.action.lower() if args.action is not None else 'empty', None)

    if action is not None:
        action()
    else:
        print 'Action is not exist'
