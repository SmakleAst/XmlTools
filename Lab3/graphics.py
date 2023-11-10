import matplotlib.pyplot as plt
import numpy as np

#Задание 1
# tmp = []
# with open('C:\\Users\\Smakl\\source\\repos\\Mirea\\TSPO\\Lab3\\Lab3\\bin\\Debug\\graphic.txt') as f:
#     for line in f:
#         tmp.append([float(x) for x in line.split()])


# y = tmp[0]

# x = np.arange(0, len(y))

# print(x)


# plt.plot(x, y)
# plt.legend(["Курс"])
# plt.show()

#Задание 2
tmp1 = []
with open('C:\\Users\\Smakl\\source\\repos\\Mirea\\TSPO\\Lab3\\Lab3\\bin\\Debug\\1.txt') as f:
    for line in f:
        tmp1.append([float(x) for x in line.split()])
tmp2 = []
with open('C:\\Users\\Smakl\\source\\repos\\Mirea\\TSPO\\Lab3\\Lab3\\bin\\Debug\\2.txt') as f:
    for line in f:
        tmp2.append([float(x) for x in line.split()])
tmp3 = []
with open('C:\\Users\\Smakl\\source\\repos\\Mirea\\TSPO\\Lab3\\Lab3\\bin\\Debug\\3.txt') as f:
    for line in f:
        tmp3.append([float(x) for x in line.split()])
tmp4 = []
with open('C:\\Users\\Smakl\\source\\repos\\Mirea\\TSPO\\Lab3\\Lab3\\bin\\Debug\\4.txt') as f:
    for line in f:
        tmp4.append([float(x) for x in line.split()])

y1 = tmp1[0]
y2 = tmp2[0]
y3 = tmp3[0]
y4 = tmp4[0]

x1 = np.arange(0, len(y1))
x2 = np.arange(0, len(y2))
x3 = np.arange(0, len(y3))
x4 = np.arange(0, len(y4))

plt.plot(x1, y1, x2, y2, x3, y3, x4, y4)
plt.legend(["Золото", "Серебро", "Платина", "Палладий"])
plt.show()