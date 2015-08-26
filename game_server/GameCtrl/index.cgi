#!/usr/bin/env python
# -*- coding: utf-8 -*-

import sys
import cgi
import os
import time
import fcntl
import json

import base64
import rijndael

import hashlib
import Crypto.Cipher.AES

def html(text):
	print "Content-Type: text/plain"
	print
	sys.stdout.write(text.encode('utf-8'))
	sys.exit()

def rcrypt(key, data, flag):
	KEY_SIZE   = 32
	BLOCK_SIZE = 32
	padded_key = key.ljust(KEY_SIZE, '\0')
	if flag:
		original = data + (BLOCK_SIZE - len(data) % BLOCK_SIZE) * '\0'
	else:
		original = base64.b64decode(data)	
	r = rijndael.rijndael(padded_key, BLOCK_SIZE)
	ret = ''
	for start in range(0, len(original), BLOCK_SIZE):
		if flag:
			ret += r.encrypt(original[start:start+BLOCK_SIZE])
		else:
			ret += r.decrypt(original[start:start+BLOCK_SIZE])
	if flag:
		ret = base64.b64encode(ret)
	else:
		ret = ret.split('\x00', 1)[0]
	return ret

def get_input():
	# get form data
	f = cgi.FieldStorage()
	if (not f.has_key("s")) or (not f.has_key("p")) or (not f.has_key("g")):
		return None
	status = f["s"].value; #status
	data   = f["p"].value; #data
	cookie = f["g"].value; #GameCookie
	# setup r (and decrypt data)
	r = {}
	r.setdefault("status", "")
	r["status"] = status
	if status == "Start":
		return r
	# GameClear
	httpkey2 = "6789ABCDEFGHIJKLMNOPQRSTUVWXYZ"
	try:
		# Size of base64.b64decode(cookie) is must 32bytes
		k = rcrypt(base64.b64decode(cookie), httpkey2, 1)
		data = rcrypt(base64.b64decode(k), data, 0)
	except:
		return None
	# data = DUMMY,p_HP,e_HP,gametime,name
	vals = data.split(",")
	if len(vals) != 5:
		return None
	r.setdefault("dummy", "")
	r["dummy"] = vals[0]
	try:
		if r["dummy"].split(":")[1] != 'g6J6KPwHXeLG+7aqoaUS+fvJDTo=':
			#if r["dummy"].split(":")[1] != 'NLcRTZheKbilckRzuwD9pp0TK/E=':
			return None
		r.setdefault("p_HP", 0)
		r["p_HP"] = int(vals[1])
		r.setdefault("e_HP", 0)
		r["e_HP"] = int(vals[2])
		r.setdefault("time", 0)
		r["time"] = int(vals[3])
	except:
		return None
	r.setdefault("name", "")
	r["name"] = base64.b64decode(vals[4])
	# add GameCookie
	r.setdefault("cookie", "")
	r["cookie"] = cookie
	# ret dict
	return r

def aes_key():
	iv = "\x00\x10\x02\x30\x04\x50\x06\x70\x08\x90\x0a\xb0\x0c\xd0\x0e\xf0"
	kkk = hashlib.sha256('Server Crypto Key for SECCON/CEDEC').digest()
	return Crypto.Cipher.AES.new(kkk, Crypto.Cipher.AES.MODE_CBC, iv)

def make_cookie():
	aes = aes_key()
	stm = "TIME:%d" % int(time.time())
	enc = aes.encrypt(os.urandom(32-len(stm)) + stm)
	return base64.b64encode(enc)

def chk_cookie(cookie):
	aes = aes_key()
	try:
		data = aes.decrypt(base64.b64decode(cookie))
		if len(data) != 32:
			return 0
		stm = "TIME:%d" % int(time.time())
		(ide, start_time) = data[32-len(stm):].split(":")
		if ide != "TIME":
			return 0
		if int(time.time()) < (int(start_time) + 330):
			return int(start_time)
	except:
		pass
	return 0

def get_time(t):
	r  = ""
	tm = time.localtime(t)
	r += "%04d/%02d/%02d-" % (tm.tm_year, tm.tm_mon, tm.tm_mday)
	r += "%02d.%02d.%02d"  % (tm.tm_hour, tm.tm_min, tm.tm_sec)
	return r

def write_data(filename, mode, data):
	with open(filename, mode) as f:
		fcntl.flock(f.fileno(), fcntl.LOCK_EX)
		try:
			f.write(json.dumps(data, indent=4) + "\n")
		finally:
			fcntl.flock(f.fileno(), fcntl.LOCK_UN)

def read_data(filename):
	return json.loads(open(filename).read())

def update_rank(filename, score, name):
	r = []
	rank = read_data(filename)
	i = 0
	for v in rank:
		if v["score"] < score:
			break
		i += 1
	if i < 10:
		d = {}
		d.setdefault("name", "")
		d["name"]  = name
		d.setdefault("score", 0)
		d["score"] = score
		d.setdefault("addr", "")
		d["addr"] = os.environ['REMOTE_ADDR']
		rank.insert(i, d)
		while 10 < len(rank):
			rank.pop();
		write_data(filename, "w", rank)

def post_main():
	# get from client
	user_info = get_input()
	if user_info == None:
		html("cheat")
	# Start
	if user_info["status"] == "Start":
		html(make_cookie())
	# GameClear
	if user_info["status"] == "GameClear":
		start_time = chk_cookie(user_info["cookie"])
		if start_time == 0:
			html("timeout")
		if user_info["p_HP"] < 1: # player HP
			html("unknown")
		if 0 < user_info["e_HP"]: # dragon HP (BOSS)
			html("unknown")
		clnt_time = 300 - user_info["time"]
		if (clnt_time < 0) or (300 < clnt_time):
			html("timerange")
		logdata = {}
		logdata.setdefault("user_info", {})
		logdata["user_info"] = user_info
		logdata.setdefault("meta_data", {})
		logdata["meta_data"].setdefault("start_time",  "")
		logdata["meta_data"].setdefault("finish_time", "")
		logdata["meta_data"]["start_time"]  = get_time(start_time)
		logdata["meta_data"]["finish_time"] = get_time(int(time.time()))
		logdata["meta_data"].setdefault("remote_addr", "")
		logdata["meta_data"]["remote_addr"] = os.environ['REMOTE_ADDR']
		write_data("clears.json", "a", logdata)
		update_rank("ranks.json", user_info["time"], user_info["name"])
		html("ok")
	html("err")

def get_main():
	r = "Ranking\n\n"
	rank = read_data("ranks.json")
	i = 1
	for v in rank:
		r += " %02d" % i
		r += ": " + str(v["score"]) + "pt, " + v["name"] + "\n"
		i += 1
	html(r)
	
if __name__ == "__main__":
	if os.environ["SERVER_PORT"] != "443":
		html("Warrior")
	# HTTPS only
	if os.environ["REQUEST_METHOD"] == "POST":
		post_main()
	else:
		get_main()

