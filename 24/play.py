# (symbol, function)
operators = [('+', lambda x, y: x + y),
             ('-', lambda x, y: x - y),
             ('*', lambda x, y: x * y),
             ('/', lambda x, y: x // y if y else None)
            ]
def play(values):
    for winner in _play_internal([(v, f'{v}') for v in values]):
        yield winner

def _play_internal(vf):
    N = len(vf)
    if N == 1 and vf[0][0] == 24:
        yield vf[0][1]
    for i in range(N):
        for j in range(i + 1, N):
            for op in operators:
                symbol = op[0]
                func = op[1]
                child = (func(vf[i][0], vf[j][0]), f'({vf[i][1]} {symbol} {vf[j][1]})')
                if child[0] is not None:
                    wf = vf.copy()
                    wf.remove(vf[i])
                    wf.remove(vf[j])
                    wf.append(child)
                    for winner in _play_internal(wf):
                        yield winner

for winner in play([4, 7, 8, 8]):
    print(winner)
